import api from './api';
import type { Message } from '../types';

export const messageService = {
  async getConversations(): Promise<string[]> {
    const response = await api.get<string[]>('/Messages/conversations');
    return response.data;
  },

  async getMessages(conversationWith: string): Promise<Message[]> {
    const response = await api.get<Message[]>(`/Messages/${conversationWith}`);
    const myEmail = localStorage.getItem('consilium_email') || '';
    
    return response.data.map(msg => ({
      ...msg,
      timeSent: new Date(msg.timeSent),
      isMyMessage: msg.sender === myEmail,
    }));
  },

  async sendMessage(message: Message): Promise<boolean> {
    try {
      await api.post('/Messages', message);
      return true;
    } catch {
      return false;
    }
  },

  async checkUser(email: string): Promise<boolean> {
    try {
      const response = await api.get(`/Messages/checkuser/${email}`);
      return response.status === 200;
    } catch {
      return false;
    }
  },

  async sendFeedback(feedback: string): Promise<boolean> {
    try {
      const response = await api.post('/NewFeature/feedback', { feedback });
      return response.status === 200;
    } catch (error) {
      console.error('Failed to send feedback:', error);
      return false;
    }
  },
};
