import { NavLink } from 'react-router-dom';
import { useState } from 'react';

interface LayoutProps {
  children: React.ReactNode;
}

export const Layout: React.FC<LayoutProps> = ({ children }) => {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  return (
    <div className="min-h-screen flex flex-col w-full">
      <nav className="bg-mid-green text-white shadow-md">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between h-16">
            <div className="flex items-center">
              <span className="text-xl sm:text-2xl font-bold">Consilium</span>
            </div>
            
            {/* Desktop Navigation */}
            <div className="hidden lg:flex gap-2 xl:gap-4">
              <NavLink 
                to="/" 
                className={({ isActive }) => 
                  `px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Dashboard
              </NavLink>
              <NavLink 
                to="/todos" 
                className={({ isActive }) => 
                  `px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Todo List
              </NavLink>
              <NavLink 
                to="/assignments" 
                className={({ isActive }) => 
                  `px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Assignments
              </NavLink>
              <NavLink 
                to="/tools" 
                className={({ isActive }) => 
                  `px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Tools
              </NavLink>
              <NavLink 
                to="/profile" 
                className={({ isActive }) => 
                  `px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Profile
              </NavLink>
              <NavLink 
                to="/settings" 
                className={({ isActive }) => 
                  `px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Settings
              </NavLink>
            </div>

            {/* Mobile menu button */}
            <button
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              className="lg:hidden p-2 rounded-md hover:bg-white/10 transition-colors"
              aria-label="Toggle menu"
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                {isMobileMenuOpen ? (
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                ) : (
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
                )}
              </svg>
            </button>
          </div>

          {/* Mobile Navigation Menu */}
          {isMobileMenuOpen && (
            <div className="lg:hidden pb-4 space-y-1">
              <NavLink 
                to="/" 
                onClick={() => setIsMobileMenuOpen(false)}
                className={({ isActive }) => 
                  `block px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Dashboard
              </NavLink>
              <NavLink 
                to="/todos" 
                onClick={() => setIsMobileMenuOpen(false)}
                className={({ isActive }) => 
                  `block px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Todo List
              </NavLink>
              <NavLink 
                to="/assignments" 
                onClick={() => setIsMobileMenuOpen(false)}
                className={({ isActive }) => 
                  `block px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Assignments
              </NavLink>
              <NavLink 
                to="/tools" 
                onClick={() => setIsMobileMenuOpen(false)}
                className={({ isActive }) => 
                  `block px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Tools
              </NavLink>
              <NavLink 
                to="/profile" 
                onClick={() => setIsMobileMenuOpen(false)}
                className={({ isActive }) => 
                  `block px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Profile
              </NavLink>
              <NavLink 
                to="/settings" 
                onClick={() => setIsMobileMenuOpen(false)}
                className={({ isActive }) => 
                  `block px-3 py-2 rounded-md text-sm font-semibold transition-colors ${
                    isActive 
                      ? 'bg-dark-green text-white' 
                      : 'text-white/80 hover:bg-white/10'
                  }`
                }
              >
                Settings
              </NavLink>
            </div>
          )}
        </div>
      </nav>
      <main className="flex-1 w-full">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
          {children}
        </div>
      </main>
    </div>
  );
};
