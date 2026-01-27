import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { Theme, Note } from '../types';

interface AppState {
  theme: Theme;
  notes: Note;
  setTheme: (theme: Theme) => void;
  setNotes: (notes: Note) => void;
}

export const useAppStore = create<AppState>()(
  persist(
    (set) => ({
      theme: 'Green',
      notes: { title: '', content: '' },
      setTheme: (theme) => set({ theme }),
      setNotes: (notes) => set({ notes }),
    }),
    {
      name: 'consilium-storage',
    }
  )
);
