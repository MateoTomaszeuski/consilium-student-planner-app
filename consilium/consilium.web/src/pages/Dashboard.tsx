import { useEffect, useState, useCallback } from 'react';
import { useAuth } from '../hooks/useAuth';
import { todoService } from '../services/todoService';
import { assignmentService } from '../services/assignmentService';
import type { Assignment, TodoItem } from '../types';

export const Dashboard = () => {
  const { user, isAuthenticated } = useAuth();
  const [username, setUsername] = useState('Guest');
  const [online, setOnline] = useState(false);
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [todos, setTodos] = useState<TodoItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showDashboard, setShowDashboard] = useState(false);

  const initialize = useCallback(async () => {
    setIsLoading(true);
    
    if (user) {
      setUsername(user.displayName || user.email.split('@')[0]);
      setOnline(true);
    } else {
      setUsername('Guest');
      setOnline(false);
    }

    if (isAuthenticated && user) {
      try {
        const allAssignments = await assignmentService.getAllAssignments();
        const upcomingAssignments = allAssignments
          .slice(0, 3)
          .sort((a, b) => {
            if (!a.dueDate) return 1;
            if (!b.dueDate) return -1;
            return a.dueDate.getTime() - b.dueDate.getTime();
          });
        setAssignments(upcomingAssignments);

        const allTodos = await todoService.getTodos();
        const incompleteTodos = allTodos
          .filter(t => !t.completionDate)
          .sort((a, b) => b.id - a.id)
          .slice(0, 5);
        setTodos(incompleteTodos);

        setShowDashboard(true);
      } catch (error) {
        console.error('Failed to load dashboard data:', error);
      }
    }

    setIsLoading(false);
  }, [user, isAuthenticated]);

  useEffect(() => {
    initialize();
  }, [initialize]);
  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-100">
        <div className="text-xl">Loading...</div>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      <h1 className="text-4xl font-bold text-center text-dark-dark">
        Welcome back, {username}!
      </h1>

      {!online && (
        <div className="bg-light-med border border-dark-med/30 rounded-lg p-6 text-center max-w-3xl mx-auto">
          <p className="text-dark-dark">
            You are in Guest mode. To use all features, please make sure you're logged in
            and connected to the internet.
          </p>
        </div>
      )}

      {showDashboard && (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          <section className="bg-light-light border border-dark-med/20 rounded-xl p-6 shadow-sm">
            <h2 className="text-2xl font-bold mb-4 text-dark-dark">Upcoming Assignments</h2>
            {assignments.length === 0 ? (
              <p className="text-dark-med text-center py-8">No upcoming assignments</p>
            ) : (
              <div className="space-y-4">
                {assignments.map((assignment) => (
                  <div 
                    key={assignment.id} 
                    className="bg-white border border-dark-med/10 rounded-lg p-4 hover:shadow-md transition-shadow"
                  >
                    <h3 className="font-bold text-lg text-dark-dark">{assignment.name}</h3>
                    {assignment.description && (
                      <p className="text-dark-med text-sm mt-2 line-clamp-2">
                        {assignment.description}
                      </p>
                    )}
                    {assignment.dueDate && (
                      <p className="text-dark-med text-sm mt-2 text-right">
                        Due: {assignment.dueDate.toLocaleDateString('en-US', {
                          month: 'short',
                          day: 'numeric',
                          year: 'numeric',
                        })}
                      </p>
                    )}
                  </div>
                ))}
              </div>
            )}
          </section>

          <section className="bg-light-light border border-dark-med/20 rounded-xl p-6 shadow-sm">
            <h2 className="text-2xl font-bold mb-4 text-dark-dark">Incomplete To-Dos</h2>
            {todos.length === 0 ? (
              <p className="text-dark-med text-center py-8">No incomplete todos</p>
            ) : (
              <div className="space-y-4">
                {todos.map((todo) => (
                  <div 
                    key={todo.id} 
                    className="bg-white border border-dark-med/10 rounded-lg p-4 hover:shadow-md transition-shadow"
                  >
                    <h3 className="font-semibold text-dark-dark">{todo.title}</h3>
                    <span className="inline-block mt-2 bg-mid-green text-white px-3 py-1 rounded-full text-xs">
                      {todo.category}
                    </span>
                  </div>
                ))}
              </div>
            )}
          </section>
        </div>
      )}
    </div>
  );
};
