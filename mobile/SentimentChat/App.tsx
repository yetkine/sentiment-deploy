import React, { useState } from 'react';
import {
  SafeAreaView,
  View,
  Text,
  TextInput,
  TouchableOpacity,
  FlatList,
  StyleSheet,
} from 'react-native';
import axios from 'axios';

// iOS Simulator için 127.0.0.1 uygundur. (Android emulator olursa 10.0.2.2)
const BASE_URL = 'http://127.0.0.1:5104';

type Msg = {
  id: number;
  text: string;
  sentimentLabel: 'positive' | 'neutral' | 'negative';
  sentimentScore: number;
  createdAt: string;
};

export default function App() {
  const [messages, setMessages] = useState<Msg[]>([]);
  const [text, setText] = useState('');
  const userId = 1; // Swagger’da oluşturduğun kullanıcı id’si

  const sendMessage = async () => {
    const t = text.trim();
    if (!t) return;

    try {
      // .NET API: POST /api/Messages  -> CreateMessageRequest { userId, text }
      const { data } = await axios.post<Msg>(
        `${BASE_URL}/api/Messages`,
        {
          userId,
          text: t,
        },
        {
          headers: {
            'Content-Type': 'application/json',
            Accept: 'application/json',
          },
        },
      );

      setMessages(prev => [...prev, data]);
      setText('');
    } catch (err: any) {
      console.warn('sendMessage error:', err?.message || err);
    }
  };

  const renderItem = ({ item }: { item: Msg }) => (
    <View style={styles.bubble}>
      <Text style={styles.msgText}>{item.text}</Text>
      <Text style={styles.meta}>
        🧠 {item.sentimentLabel} ({(item.sentimentScore * 100).toFixed(1)}%)
      </Text>
    </View>
  );

  return (
    <SafeAreaView style={styles.safe}>
      <Text style={styles.title}>AI Chat + Sentiment</Text>

      <FlatList
        style={styles.list}
        data={messages}
        keyExtractor={m => String(m.id)}
        renderItem={renderItem}
      />

      <View style={styles.inputRow}>
        <TextInput
          style={styles.input}
          value={text}
          onChangeText={setText}
          placeholder="Type your message..."
          placeholderTextColor="#888"
        />
        <TouchableOpacity style={styles.button} onPress={sendMessage}>
          <Text style={styles.buttonText}>Send</Text>
        </TouchableOpacity>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: { flex: 1, backgroundColor: '#0b1220' },
  title: {
    color: 'white',
    fontWeight: '700',
    fontSize: 20,
    textAlign: 'center',
    marginVertical: 12,
  },
  list: { flex: 1, paddingHorizontal: 16 },
  bubble: {
    backgroundColor: '#141d2f',
    padding: 12,
    borderRadius: 10,
    marginVertical: 6,
  },
  msgText: { color: 'white', fontSize: 16, marginBottom: 4 },
  meta: { color: '#c7d2fe' },
  inputRow: { flexDirection: 'row', padding: 12, gap: 8 },
  input: {
    flex: 1,
    backgroundColor: '#111827',
    color: 'white',
    borderRadius: 8,
    paddingHorizontal: 12,
    height: 44,
  },
  button: {
    backgroundColor: '#f97316',
    height: 44,
    borderRadius: 8,
    alignItems: 'center',
    justifyContent: 'center',
    paddingHorizontal: 16,
  },
  buttonText: { color: 'white', fontWeight: '700' },
});
