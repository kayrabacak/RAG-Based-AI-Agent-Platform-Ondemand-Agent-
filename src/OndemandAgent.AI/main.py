from fastapi import FastAPI, Depends, HTTPException
from pydantic import BaseModel
from sqlalchemy.orm import Session
from database import engine, get_db
import models
from sqlalchemy import text
import rag_service 
import os

with engine.connect() as connection:
    connection.execute(text("CREATE EXTENSION IF NOT EXISTS vector"))
    connection.commit()

models.Base.metadata.create_all(bind=engine)

app = FastAPI()

class IndexRequest(BaseModel):
    event_id: str
    document_id: str
    file_path: str 

class ChatRequest(BaseModel):
    event_id: str
    question: str

@app.get("/")
def read_root():
    return {"message": "Python AI Servisi Aktif (Gemini Powered) 🚀"}

@app.get("/db-test")
def test_db_connection(db: Session = Depends(get_db)):
    try:
        db.execute(text("SELECT 1"))
        return {"status": "ok", "message": "Veritabanı bağlantısı başarılı."}
    except Exception as e:
        return {"status": "error", "message": str(e)}

@app.post("/index-document")
def index_document(request: IndexRequest, db: Session = Depends(get_db)):
    try:
        if not os.path.exists(request.file_path):
             return {"status": "error", "message": f"Dosya bulunamadı: {request.file_path}"}

        chunk_count = rag_service.process_and_index_pdf(
            file_path=request.file_path,
            event_id=request.event_id,
            document_id=request.document_id,
            db=db
        )

        return {
            "status": "success", 
            "message": f"Doküman başarıyla işlendi. {chunk_count} parça vektör veritabanına kaydedildi."
        }
    
    except Exception as e:
        print(f"INDEX HATA: {str(e)}")
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/chat")
def chat_endpoint(request: ChatRequest, db: Session = Depends(get_db)):
    try:
        answer = rag_service.ask_question(
            question=request.question,
            event_id=request.event_id,
            db=db
        )
        return {"answer": answer}
    
    except Exception as e:
        print(f"CHAT HATA: {str(e)}")
        return {"answer": "Üzgünüm, şu an sistemsel bir hata nedeniyle cevap veremiyorum. Lütfen tekrar deneyin."}