import React, { useState, useEffect } from "react";
import api from "./api"; // ← axios yerine bunu kullan

function App() {
  const [messages, setMessages] = useState([]);
  const [input, setInput] = useState("");
  const userId = 1; // demo kullanıcı

  // (opsiyonel) mevcut mesajları yükle
  useEffect(() => {
    (async () => {
      try {
        const res = await api.get(`/api/messages/by-user/${userId}`);
        setMessages(res.data ?? []);
      } catch (e) {
        console.error("fetch messages failed", e);
      }
    })();
  }, []);

  const sendMessage = async () => {
    if (!input.trim()) return;
    try {
      const res = await api.post("/api/messages", {
        userId,
        text: input,
      });
      setMessages((prev) => [...prev, res.data]);
      setInput("");
    } catch (e) {
      console.error("send message failed", e);
      alert("Mesaj gönderilemedi. Konsola bak.");
    }
  };

  return (
    <div style={{ maxWidth: 600, margin: "auto", padding: 20 }}>
      <h2>💬 AI Chat + Sentiment</h2>
      <div
        style={{
          border: "1px solid #ccc",
          padding: 10,
          height: 400,
          overflowY: "auto",
        }}
      >
        {messages.map((m) => (
          <div key={m.id} style={{ marginBottom: 10 }}>
            <b>{m.text}</b>
            <div>
              🧠 <i>{m.sentimentLabel}</i> (
              {(m.sentimentScore * 100).toFixed(1)}%)
            </div>
          </div>
        ))}
      </div>
      <div style={{ marginTop: 10 }}>
        <input
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Type your message..."
          style={{ width: "80%", padding: 8 }}
        />
        <button onClick={sendMessage} style={{ padding: 8 }}>
          Send
        </button>
      </div>
    </div>
  );
}

export default App;
