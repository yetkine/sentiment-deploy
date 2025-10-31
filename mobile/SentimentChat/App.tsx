import React, { useState } from 'react';
import {
  SafeAreaView,
  View,
  Text,
  TextInput,
  TouchableOpacity,
  FlatList,
  StyleSheet,
  ActivityIndicator,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';
import api from './src/services/api';

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
  const [sending, setSending] = useState(false);
  const userId = 1; // seed user

  const sendMessage = async () => {
    const t = text.trim();
    if (!t || sending) return;

    setSending(true);
    try {
      const { data } = await api.post<Msg>('/messages', { userId, text: t });
      setMessages(prev => [...prev, data]);
      setText('');
    } catch (err: any) {
      console.warn('sendMessage error:', err?.message || err);
    } finally {
      setSending(false);
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
      <KeyboardAvoidingView
        style={{ flex: 1 }}
        behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      >
        <Text style={styles.title}>AI Chat + Sentiment</Text>

        <FlatList
          style={styles.list}
          data={messages}
          keyExtractor={m => String(m.id)}
          renderItem={renderItem}
          contentContainerStyle={{ paddingHorizontal: 16, paddingBottom: 8 }}
          keyboardShouldPersistTaps="handled"
        />

        <View style={styles.inputRow}>
          <TextInput
            style={styles.input}
            value={text}
            onChangeText={setText}
            placeholder="Type your message..."
            placeholderTextColor="#888"
            editable={!sending}
          />
          <TouchableOpacity
            style={[styles.button, sending && { opacity: 0.6 }]}
            onPress={sendMessage}
            disabled={sending}
          >
            {sending ? (
              <ActivityIndicator color="#fff" />
            ) : (
              <Text style={styles.buttonText}>Send</Text>
            )}
          </TouchableOpacity>
        </View>
      </KeyboardAvoidingView>
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
  list: { flex: 1 },
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
