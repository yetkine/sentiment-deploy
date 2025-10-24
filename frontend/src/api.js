// src/api.js
import axios from "axios";

// CRA: process.env.REACT_APP_*
const raw =
  process.env.REACT_APP_API_BASE || "https://sentiment-chat-api.onrender.com";
const baseURL = raw.replace(/\/+$/, "");

const api = axios.create({
  baseURL: `${baseURL}/api`,
});

export default api;
