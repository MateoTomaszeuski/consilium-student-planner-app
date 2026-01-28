import { useContext } from 'react';
import { AuthContext } from '../contexts/GoogleAuthContext';

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within a GoogleAuthProvider');
  }
  return context;
}
