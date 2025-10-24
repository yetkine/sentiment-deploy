# app.py
import os
# HF Spaces'ta yazılabilir cache klasörü kullan
os.makedirs("/cache", exist_ok=True)
os.environ["HF_HOME"] = "/cache"                 # Hugging Face cache kökü
os.environ["TRANSFORMERS_CACHE"] = "/cache/hf"   # Transformers cache
os.environ["VECLIB_MAXIMUM_THREADS"] = "1"
os.environ["OMP_NUM_THREADS"] = "1"
os.environ["PYTORCH_ENABLE_MPS_FALLBACK"] = "1"
os.environ["TOKENIZERS_PARALLELISM"] = "false"

from fastapi import FastAPI, Request
from pydantic import BaseModel
from typing import Literal
import torch
from transformers import (
    AutoTokenizer,
    AutoModelForSequenceClassification,
    TextClassificationPipeline,
)

MODEL_ID = "cardiffnlp/twitter-xlm-roberta-base-sentiment"  # 0=neg,1=neu,2=pos

app = FastAPI(title="Sentiment Service", version="1.0")

class AnalyzeRequest(BaseModel):
    text: str

class AnalyzeResponse(BaseModel):
    label: Literal["positive", "neutral", "negative"]
    score: float

_tokenizer = None
_model = None
_pipeline = None

@app.on_event("startup")
def load_model():
    """Model ve tokenizer'ı bir kez yükle."""
    global _tokenizer, _model, _pipeline
    torch.set_num_threads(1)
    torch.set_num_interop_threads(1)

    _tokenizer = AutoTokenizer.from_pretrained(MODEL_ID, use_fast=True)
    _model = AutoModelForSequenceClassification.from_pretrained(MODEL_ID)
    _pipeline = TextClassificationPipeline(
        model=_model,
        tokenizer=_tokenizer,
        device=-1,  # CPU
        framework="pt",
        return_all_scores=False,
        task="sentiment-analysis",
    )

@app.get("/")
def root():
    return {"message": "Sentiment API is running. Use POST /analyze."}

@app.get("/health")
def health():
    return {"status": "ok"}

@app.post("/analyze", response_model=AnalyzeResponse)
def analyze(req: AnalyzeRequest):
    result = _pipeline(req.text)[0]  # {'label': 'LABEL_2', 'score': 0.95}
    raw_label = result["label"]
    score = float(result["score"])

    label_map = {
        "LABEL_0": "negative",
        "LABEL_1": "neutral",
        "LABEL_2": "positive",
        "negative": "negative",
        "neutral": "neutral",
        "positive": "positive",
    }
    label = label_map.get(raw_label, "neutral")
    return {"label": label, "score": round(score, 4)}
