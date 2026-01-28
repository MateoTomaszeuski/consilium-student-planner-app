import { useEffect, useState, useCallback } from 'react';
import { todoService } from '../services/todoService';
import { useAuth } from '../hooks/useAuth';
import type { TodoItem } from '../types';

const CATEGORIES = ['Misc.', 'School', 'Work'];
const SORT_OPTIONS = [
  'Category A-Z',
  'Category: Z-A',
  'Title: A-Z',
  'Title: Z-A',
  'Completion',
];

export const TodoList = () => {
  const { isAuthenticated } = useAuth();
  const [todos, setTodos] = useState<TodoItem[]>([]);
  const [newTodoTitle, setNewTodoTitle] = useState('');
  const [newTodoCategory, setNewTodoCategory] = useState(CATEGORIES[0]);
  const [selectedCategory, setSelectedCategory] = useState('All');
  const [sortOption, setSortOption] = useState(SORT_OPTIONS[0]);
  const [isLoading, setIsLoading] = useState(true);
  const [message, setMessage] = useState('');

  const loadTodos = useCallback(async () => {
    setIsLoading(true);
    
    if (!isAuthenticated) {
      setIsLoading(false);
      return;
    }
    
    try {
      const data = await todoService.getTodos();
      setTodos(data);
      setMessage(data.length < 1 ? 'No items found.' : '');
    } catch (error) {
      console.error('Failed to load todos:', error);
      setMessage('Failed to load todos');
    }
    setIsLoading(false);
  }, []);

  const applyFiltersAndSort = useCallback(() => {
    // This would be implemented with proper filtering and sorting logic
    // For now, just keeping the original array
  }, []);

  useEffect(() => {
    loadTodos();
  }, [loadTodos]);

  useEffect(() => {
    applyFiltersAndSort();
  }, [selectedCategory, sortOption, applyFiltersAndSort]);

  const addTodo = async () => {
    if (!newTodoTitle.trim()) return;

    try {
      await todoService.addTodo({
        title: newTodoTitle,
        category: newTodoCategory,
        todoListId: 1, // Default list ID
        isCompleted: false,
      });
      setNewTodoTitle('');
      await loadTodos();
    } catch (error) {
      console.error('Failed to add todo:', error);
    }
  };

  const toggleComplete = async (todo: TodoItem) => {
    try {
      const updated = {
        ...todo,
        isCompleted: !todo.isCompleted,
        completionDate: !todo.isCompleted ? new Date() : undefined,
      };
      await todoService.updateTodo(updated);
      await loadTodos();
    } catch (error) {
      console.error('Failed to update todo:', error);
    }
  };

  const deleteTodo = async (id: number) => {
    try {
      await todoService.deleteTodo(id);
      await loadTodos();
    } catch (error) {
      console.error('Failed to delete todo:', error);
    }
  };

  const clearCompleted = async () => {
    const completed = todos.filter(t => t.isCompleted);
    for (const todo of completed) {
      await deleteTodo(todo.id);
    }
  };

  if (isLoading) {
    return <div className="flex items-center justify-center min-h-100 text-xl">Loading todos...</div>;
  }

  if (!isAuthenticated) {
    return (
      <div className="space-y-8">
        <h1 className="text-3xl font-bold text-center text-dark-dark">Todo List</h1>
        <div className="bg-light-med border border-dark-med/30 rounded-lg p-6 text-center max-w-3xl mx-auto">
          <p className="text-dark-dark">
            You are in Guest mode. To use all features, please go to the Profile page and log in.
          </p>
        </div>
      </div>
    );
  }

  const hasCompletedTasks = todos.some(t => t.isCompleted);

  return (
    <div className="max-w-4xl mx-auto">
      <h1 className="text-3xl font-bold mb-8 text-dark-dark">Todo List</h1>

      <div className="space-y-4 mb-8">
        <div className="flex gap-2 flex-col sm:flex-row">
          <input
            type="text"
            placeholder="New todo..."
            value={newTodoTitle}
            onChange={(e) => setNewTodoTitle(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && addTodo()}
            className="flex-1 px-4 py-2 border border-dark-med/30 rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-mid-green"
          />
          <select
            value={newTodoCategory}
            onChange={(e) => setNewTodoCategory(e.target.value)}
            className="px-4 py-2 border border-dark-med/30 rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-mid-green min-w-35"
          >
            {CATEGORIES.map(cat => (
              <option key={cat} value={cat}>{cat}</option>
            ))}
          </select>
          <button onClick={addTodo} className="px-6 py-2 bg-mid-green text-white rounded-lg hover:bg-dark-green transition-colors font-semibold">Add</button>
        </div>

        <div className="flex gap-2 flex-wrap">
          <select
            value={selectedCategory}
            onChange={(e) => setSelectedCategory(e.target.value)}
            className="px-4 py-2 border border-dark-med/30 rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-mid-green"
          >
            <option value="All">All Categories</option>
            {CATEGORIES.map(cat => (
              <option key={cat} value={cat}>{cat}</option>
            ))}
          </select>

          <select
            value={sortOption}
            onChange={(e) => setSortOption(e.target.value)}
            className="px-4 py-2 border border-dark-med/30 rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-mid-green"
          >
            {SORT_OPTIONS.map(option => (
              <option key={option} value={option}>{option}</option>
            ))}
          </select>

          {hasCompletedTasks && (
            <button onClick={clearCompleted} className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors font-semibold">
              Clear Completed
            </button>
          )}
        </div>
      </div>

      {message && <p className="text-center text-dark-med py-4">{message}</p>}

      <div className="space-y-3">
        {todos.map(todo => (
          <div key={todo.id} className={`flex items-center gap-4 p-4 bg-light-light border border-dark-med/20 rounded-lg hover:shadow-md transition-shadow ${todo.isCompleted ? 'opacity-60' : ''}`}>
            <input
              type="checkbox"
              checked={todo.isCompleted}
              onChange={() => toggleComplete(todo)}
              className="w-5 h-5 cursor-pointer accent-mid-green"
            />
            <div className="flex-1">
              <span className={`block text-dark-dark ${todo.isCompleted ? 'line-through' : ''}`}>
                {todo.title}
              </span>
              <span className="text-sm text-dark-med">{todo.category}</span>
            </div>
            <button 
              onClick={() => deleteTodo(todo.id)} 
              className="px-4 py-2 text-red-600 border border-red-600 rounded-lg hover:bg-red-600 hover:text-white transition-colors"
            >
              Delete
            </button>
          </div>
        ))}
      </div>
    </div>
  );
};
