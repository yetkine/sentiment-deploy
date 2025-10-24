# Sentiment Chat (Web + Mobile + .NET API + AI Service)

## Genel

Kullanıcıların mesajlaşıp **anlık duygu skoru** (pozitif/nötr/negatif) görebildiği basit bir sistem.

- **Frontend (Web):** React (Vite) – `frontend/`
- **Mobile (Opsiyonel):** React Native – `mobile/SentimentChat/`
- **Backend API:** .NET 8 / EF Core / SQLite – `backend/Chat.Api/`
- **AI Servisi:** FastAPI + Hugging Face Transformers – `ai-service/`

---

## Çalışır Demo Linkleri

- **Web (Vercel):** https://<senin-vercel-projen>.vercel.app
- **Mobil Build:** (APK ya da iOS TestFlight/AdHoc linki veya kısa video/gif)
- **AI Endpoint (HF Space):** https://<senin-space-adin>.hf.space/analyze
- **Backend (Render):** https://<senin-render-app>.onrender.com

> Not: Frontend `.env` içinde `VITE_API_BASE` Render API URL’sine işaret eder.

---

## Kurulum (Local)

### 1) AI Servis (FastAPI)

```bash
cd ai-service
# (opsiyonel sanal ortam) python -m venv .venv && source .venv/bin/activate
pip install -r requirements.txt
uvicorn app:app --host 127.0.0.1 --port 8000 --reload
# test:
curl -X POST http://127.0.0.1:8000/analyze -H "Content-Type: application/json" -d '{"text":"i feel great"}'
```

### 2) Backend (.NET)

cd backend/Chat.Api
dotnet restore
dotnet ef database update # yoksa app açılışında migrate eder
dotnet run

# Swagger: http://localhost:5104/swagger

appsettings.json (özet):

{
"ConnectionStrings": { "Default": "Data Source=chat.db" },
"AiService": { "BaseUrl": "http://127.0.0.1:8000", "Endpoint": "/analyze" },
"Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
"AllowedHosts": "\*"
}

### 3) Frontend (Web)

cd frontend
npm i
echo 'VITE_API_BASE=http://localhost:5104' > .env
npm run dev # http://localhost:5173
