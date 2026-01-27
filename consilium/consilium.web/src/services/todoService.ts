import api from './api';
import type { TodoItem } from '../types';

export const todoService = {
  async getTodos(): Promise<TodoItem[]> {
    const response = await api.get<TodoItem[]>('/Todo');
    return response.data.map(todo => ({
      ...todo,
      completionDate: todo.completionDate ? new Date(todo.completionDate) : undefined,
      isCompleted: !!todo.completionDate,
      subtasks: todo.subtasks || [],
    }));
  },

  async addTodo(todo: Omit<TodoItem, 'id' | 'subtasks'>): Promise<void> {
    await api.post('/Todo', todo);
  },

  async updateTodo(todo: TodoItem): Promise<void> {
    await api.put(`/Todo/${todo.id}`, todo);
  },

  async deleteTodo(id: number): Promise<void> {
    await api.delete(`/Todo/${id}`);
  },

  async addSubtask(parentId: number, subtask: Omit<TodoItem, 'id' | 'subtasks'>): Promise<void> {
    await api.post(`/Todo/${parentId}/subtask`, subtask);
  },
};
