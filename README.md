# OndemandAgent

**OndemandAgent** is a comprehensive RAG-based AI Agent platform designed to provide intelligent, context-aware responses by leveraging advanced retrieval-augmented generation techniques. The system is built with a microservices-inspired architecture, integrating a high-performance .NET Core API, a flexible Python-based AI engine, and a modern Blazor user interface.

## 🚀 Features

-   **RAG Architecture**: Retrieves relevant documents to augment LLM generation for accurate and context-specific answers.
-   **Multi-Agent System**: Orchestrates specialized agents for different tasks (e.g., search, summarization, creative writing).
-   **Vector Search**: Utilizes PostgreSQL with `pgvector` for efficient semantic search and retrieval.
-   **Modern UI**: A responsive and interactive dashboard built with Blazor for managing agents and viewing conversations.
-   **Secure**: Integrated JWT authentication for secure API access.
-   **Scalable**: Containerized with Docker for easy deployment and scalability.

## 🛠️ Technology Stack

-   **Backend**: ASP.NET Core 8 Web API
-   **AI Engine**: Python (FastAPI, LangGraph, OpenAI, Pydantic)
-   **Frontend**: Blazor WebAssembly / Server
-   **Database**: PostgreSQL (with `pgvector` extension)
-   **Containerization**: Docker & Docker Compose

## 📂 Project Structure

```bash
OndemandAgent/
├── src/
│   ├── OndemandAgent.AI/       # Python AI Service & LangGraph Logic
│   ├── OndemandAgent.Web/      # ASP.NET Core Web API Backend
│   └── OndemandAgent.UI/       # Blazor Frontend Application
├── docker-compose.yml          # Docker composition for database services
└── README.md                   # Project Documentation
```

## 🏁 Getting Started

Follow these instructions to get the project up and running on your local machine.

### Prerequisites

-   [Docker Desktop](https://www.docker.com/products/docker-desktop)
-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Python 3.10+](https://www.python.org/downloads/)
-   [Node.js](https://nodejs.org/) (Optional, for frontend tooling)

### 1. Database Setup

Start the PostgreSQL database with the `pgvector` extension using Docker Compose.

```bash
docker-compose up -d
```

This will start a PostgreSQL container on port `5432`.

### 2. AI Service Setup (Python)

Navigate to the AI service directory and set up the Python environment.

```bash
cd src/OndemandAgent.AI

# Create a virtual environment
python -m venv venv

# Activate the virtual environment
# Windows:
.\venv\Scripts\activate
# macOS/Linux:
source venv/bin/activate

# Install dependencies (ensure requirements.txt exists or install manually)
pip install -r requirements.txt  # If requirements.txt is present
# Or install core packages:
pip install fastapi uvicorn langgraph openai pydantic pgvector

# Run the AI Service
uvicorn main:app --reload --port 8000
```

### 3. Backend API Setup (.NET)

Navigate to the Web API directory.

```bash
cd src/OndemandAgent.Web

# Update appsettings.json with your specific configuration (Database QueryStrings, API Keys)

# Restore dependencies
dotnet restore

# Apply Database Migrations
dotnet ef database update

# Run the API
dotnet run
```

The API will typically start on `http://localhost:5000` or `https://localhost:5001`.

### 4. Frontend UI Setup (Blazor)

Navigate to the UI directory.

```bash
cd src/OndemandAgent.UI

# Restore dependencies
dotnet restore

# Run the Frontend
dotnet run
```

## ⚙️ Configuration

-   **Environment Variables**: extensive configuration is managed via `appsettings.json` in the .NET projects and `.env` files for Python services (make sure to create `.env` from `.env.example` if available).
-   **Database Connection**: Ensure the connection string in `src/OndemandAgent.Web/appsettings.json` matches your Docker container settings.

## 🤝 Contributing

Contributions are welcome! Please fork the repository and submit a pull request for any enhancements or bug fixes.

## 📄 License

This project is licensed under the MIT License.