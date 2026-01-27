// Core types based on the C# models

export interface User {
  id: number;
  email: string;
  displayName: string;
}

export interface Course {
  id: number;
  courseName: string;
}

export interface Assignment {
  id: number;
  name: string;
  description?: string;
  courseId: number;
  dueDate?: Date;
  dateStarted?: Date;
  dateCompleted?: Date;
  isCompleted: boolean;
  descriptionIsExpanded?: boolean;
}

export interface TodoItem {
  id: number;
  parentId?: number;
  title: string;
  todoListId: number;
  assignmentId?: number;
  completionDate?: Date;
  category: string;
  isExpanded?: boolean;
  subtaskEntryIsVisible?: boolean;
  isCompleted: boolean;
  subtasks: TodoItem[];
}

export interface Message {
  sender: string;
  receiver: string;
  content: string;
  timeSent: Date;
  isMyMessage?: boolean;
}

export interface Note {
  title?: string;
  content?: string;
}

export type Theme = 'Green' | 'Blue' | 'Purple' | 'Pink' | 'Teal';

export interface ToDoList {
  id: number;
  name: string;
}
