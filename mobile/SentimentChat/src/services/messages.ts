// src/services/messages.ts
import { api } from './api';

export type Sentiment = 'positive' | 'neutral' | 'negative' | 'unknown';

export interface MessageDto {
  id: number;
  userId: number;
  text: string;
  sentimentLabel: Sentiment;
  sentimentScore: number;
  createdAt: string;
}

export async function sendMessage(
  userId: number,
  text: string,
): Promise<MessageDto> {
  const res = await api.post<MessageDto>('/messages', { userId, text });
  return res.data;
}

export async function listMessagesByUser(
  userId: number,
): Promise<MessageDto[]> {
  const res = await api.get<MessageDto[]>(`/messages/by-user/${userId}`);
  return res.data;
}
