import axios from 'axios';

// Canlı backend (Render)
export const BASE_URL = 'https://sentiment-chat-api.onrender.com/api';

const api = axios.create({
  baseURL: BASE_URL,
  timeout: 15000,
  headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
});

export default api;
