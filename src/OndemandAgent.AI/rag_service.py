from langchain_community.document_loaders import PyPDFLoader
from langchain_text_splitters import RecursiveCharacterTextSplitter
from langchain_huggingface import HuggingFaceEmbeddings 
from langchain_google_genai import ChatGoogleGenerativeAI, HarmBlockThreshold, HarmCategory
from sqlalchemy.orm import Session
from sqlalchemy import text
import models
import os
from dotenv import load_dotenv

load_dotenv()

print("📥 Yerel Embedding Modeli Yükleniyor... (paraphrase-multilingual-MiniLM-L12-v2)")
embeddings = HuggingFaceEmbeddings(model_name="sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2")

print("🌐 Google Gemini (gemini-2.5-pro) Hazırlanıyor...")

safety_settings = {
    HarmCategory.HARM_CATEGORY_DANGEROUS_CONTENT: HarmBlockThreshold.BLOCK_NONE,
    HarmCategory.HARM_CATEGORY_HATE_SPEECH: HarmBlockThreshold.BLOCK_NONE,
    HarmCategory.HARM_CATEGORY_HARASSMENT: HarmBlockThreshold.BLOCK_NONE,
    HarmCategory.HARM_CATEGORY_SEXUALLY_EXPLICIT: HarmBlockThreshold.BLOCK_NONE,
}

llm = ChatGoogleGenerativeAI(
    model="gemini-2.5-pro", 
    google_api_key=os.getenv("GOOGLE_API_KEY"),
    temperature=0.3,
    safety_settings=safety_settings 
)

def process_and_index_pdf(file_path: str, event_id: str, document_id: str, db: Session):
    print(f"📄 PDF İşleniyor: {file_path}")
    
    loader = PyPDFLoader(file_path)
    pages = loader.load() 
    
    text_splitter = RecursiveCharacterTextSplitter(chunk_size=1000, chunk_overlap=200)
    splits = text_splitter.split_documents(pages)
    
    print(f"🧩 Toplam Parça Sayısı: {len(splits)}")
    
    successful_chunks = 0

    for split in splits:
        text_content = split.page_content
        if not text_content.strip(): continue
        
        try:
            vector = embeddings.embed_query(text_content)
            
            db_chunk = models.DocumentChunk(
                event_id=event_id,
                document_id=document_id,
                chunk_text=text_content,
                embedding=vector 
            )
            db.add(db_chunk)
            successful_chunks += 1
        except Exception as e:
            print(f"⚠️ HATA: {str(e)}")
            continue
            
    db.commit()
    print(f"✅ İndeksleme Tamamlandı! {successful_chunks} parça kaydedildi.")
    return successful_chunks

def ask_question(question: str, event_id: str, db: Session):
    print(f"❓ Soru Geldi: {question}")

    question_vector = embeddings.embed_query(question)

    results = db.execute(
        text("""
            SELECT chunk_text 
            FROM document_chunks 
            WHERE event_id = :eid
            ORDER BY embedding <=> :vec 
            LIMIT 5
        """),
        {"eid": event_id, "vec": str(question_vector)}
    ).fetchall()

    if not results:
        context_text = ""
        print("ℹ️ Context bulunamadı, AI genel bilgiyle veya boş dönecek.")
    else:
        context_text = "\n\n".join([row[0] for row in results])
    
    prompt = f"""
    You are a helpful and polite event assistant AI.
    Answer the user's "QUESTION" using ONLY the provided "CONTEXT" information.

    RULES:
    1. Identify the language of the "QUESTION" and reply in the EXACT SAME language.
    2. If the user greets you, respond politely in the user's language.
    3. If the answer is not in the "CONTEXT", politely say that the information is not available in the documents.
    4. Answer in plain text format (no JSON, no Code blocks).
    5. Use logical reasoning. If the context prohibits an action (e.g., "No weapons allowed"), and the question is "Can I bring a gun?", answer "No" based on that prohibition.

    CONTEXT:
    {context_text}

    QUESTION: 
    {question}
    """
    
    try:
        response = llm.invoke(prompt)
        
        final_answer = response.content

        if isinstance(final_answer, list):
            text_parts = []
            for part in final_answer:
                if isinstance(part, dict) and 'text' in part:
                    text_parts.append(part['text'])
                elif isinstance(part, str):
                    text_parts.append(part)
            final_answer = " ".join(text_parts)
        
        if not isinstance(final_answer, str):
            final_answer = str(final_answer)

        if not final_answer.strip():
            return "Üzgünüm, boş cevap döndü."
            
        return final_answer
        
    except Exception as e:
        print(f"❌ Gemini Hatası: {e}")
        return "Üzgünüm, yapay zeka servisine bağlanırken bir hata oluştu."