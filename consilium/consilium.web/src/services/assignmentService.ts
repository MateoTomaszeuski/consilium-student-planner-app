import api from './api';
import type { Assignment, Course } from '../types';

export const assignmentService = {
  async getAllAssignments(): Promise<Assignment[]> {
    const response = await api.get<Assignment[]>('/Assignment');
    return response.data.map(a => ({
      ...a,
      dueDate: a.dueDate ? new Date(a.dueDate) : undefined,
      dateStarted: a.dateStarted ? new Date(a.dateStarted) : undefined,
      dateCompleted: a.dateCompleted ? new Date(a.dateCompleted) : undefined,
    }));
  },

  async getAllCourses(): Promise<Course[]> {
    const response = await api.get<Course[]>('/Assignment/courses');
    return response.data;
  },

  async addAssignment(assignment: Omit<Assignment, 'id'>): Promise<void> {
    await api.post('/Assignment', assignment);
  },

  async updateAssignment(assignment: Assignment): Promise<void> {
    await api.put(`/Assignment/${assignment.id}`, assignment);
  },

  async deleteAssignment(id: number): Promise<void> {
    await api.delete(`/Assignment/${id}`);
  },

  async addCourse(course: Omit<Course, 'id'>): Promise<void> {
    await api.post('/Assignment/course', course);
  },

  async deleteCourse(id: number): Promise<void> {
    await api.delete(`/Assignment/course/${id}`);
  },

  async startAssignment(assignment: Assignment): Promise<void> {
    const updatedAssignment = { ...assignment, dateStarted: new Date() };
    await this.updateAssignment(updatedAssignment);
  },
};
