// src/api.js
import axios from "axios";

// Vercel'de: Settings → Environment Variables → VITE_API_BASE = https://sentiment-chat-api.onrender.com
// Lokal geliştirme için istersen http://localhost:5104 yazabilirsin.
const baseURL =
  import.meta.env.VITE_API_BASE?.replace(/\/+$/, "") ||
  "https://sentiment-chat-api.onrender.com";

const api = axios.create({
  baseURL: `${baseURL}/api`,
});

export default api;
