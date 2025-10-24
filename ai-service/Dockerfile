FROM python:3.11-slim

# Sistem bağımlılıkları (opsiyonel ama faydalı)
RUN apt-get update && apt-get install -y --no-install-recommends \
    git curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Python bağımlılıkları
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Uygulama dosyaları
COPY . .

# ✅ Yazılabilir cache klasörü ayarla
ENV TRANSFORMERS_CACHE=/cache \
    HF_HOME=/cache \
    HF_HUB_CACHE=/cache/hub

# Cache klasörünü oluştur ve izin ver
RUN mkdir -p /cache && chmod -R 777 /cache

# (Opsiyonel) Build sırasında modeli “önceden” indir — ilk açılışı hızlandırır
# Ağ erişimi yoksa bu adım sessizce atlanabilir; sorun değil.
RUN python - <<'PY' || true
from transformers import AutoTokenizer, AutoModelForSequenceClassification
m = "cardiffnlp/twitter-xlm-roberta-base-sentiment"
AutoTokenizer.from_pretrained(m, cache_dir="/cache")
AutoModelForSequenceClassification.from_pretrained(m, cache_dir="/cache")
print("✅ Model prefetch ok")
PY

ENV PORT=7860
EXPOSE 7860

CMD ["uvicorn","app:app","--host","0.0.0.0","--port","7860"]
