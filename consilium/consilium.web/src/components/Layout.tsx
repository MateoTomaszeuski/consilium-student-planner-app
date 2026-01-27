import { NavLink } from 'react-router-dom';

interface LayoutProps {
  children: React.ReactNode;
}

export const Layout: React.FC<LayoutProps> = ({ children }) => {
  return (
    <div className="min-h-screen flex flex-col w-full">
      <nav className="bg-mid-green text-white shadow-md">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between h-16">
            <div className="flex items-center">
              <span className="text-2xl font-bold">Consilium</span>
            </div>
            <div className="flex flex-wrap gap-2 md:gap-4">
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
          </div>
        </div>
      </nav>
      <main className="flex-1 w-full">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          {children}
        </div>
      </main>
    </div>
  );
};
