import { useAppStore } from '../store/appStore';
import { useAuth } from '../hooks/useAuth';
import type { Theme } from '../types';

const THEMES: Theme[] = ['Green', 'Blue', 'Purple', 'Pink', 'Teal'];

export const Settings = () => {
  const { theme, setTheme } = useAppStore();
  const { isAuthenticated, updateTheme } = useAuth();

  if (!isAuthenticated) {
    return (
      <div className="space-y-6 sm:space-y-8">
        <h1 className="text-2xl sm:text-3xl font-bold text-center text-dark-dark">Settings</h1>
        <div className="bg-light-med border border-dark-med/30 rounded-lg p-4 sm:p-6 text-center max-w-3xl mx-auto">
          <p className="text-sm sm:text-base text-dark-dark">
            You are in Guest mode. To use all features, please go to the Profile page and log in.
          </p>
        </div>
      </div>
    );
  }

  const handleThemeChange = async (newTheme: Theme) => {
    setTheme(newTheme);
    await updateTheme(newTheme);
  };

  return (
    <div className="max-w-4xl mx-auto">
      <h1 className="text-2xl sm:text-3xl font-bold mb-6 sm:mb-8 text-dark-dark">Settings</h1>

      <div className="space-y-4 sm:space-y-6">
        <div className="bg-light-light border border-dark-med/20 rounded-xl p-4 sm:p-6 shadow-sm">
          <h2 className="text-xl sm:text-2xl font-bold mb-3 sm:mb-4 text-dark-dark">Theme</h2>
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-5 gap-2 sm:gap-3">
            {THEMES.map((t) => (
              <button
                key={t}
                className={`px-3 sm:px-4 py-2 sm:py-3 rounded-lg font-semibold transition-all border-2 text-sm sm:text-base ${
                  theme === t 
                    ? 'bg-mid-green text-white border-dark-green shadow-md scale-105' 
                    : 'bg-white text-dark-dark border-dark-med/20 hover:border-mid-green hover:shadow-sm'
                }`}
                onClick={() => handleThemeChange(t)}
              >
                {t}
              </button>
            ))}
          </div>
        </div>

        <div className="bg-light-light border border-dark-med/20 rounded-xl p-4 sm:p-6 shadow-sm">
          <h2 className="text-xl sm:text-2xl font-bold mb-3 sm:mb-4 text-dark-dark">Support</h2>
          <p className="text-sm sm:text-base text-dark-med">
            For recommendations, feedback, or support, please contact{' '}
            <a href="mailto:tomaszeuskim@gmail.com" className="text-mid-green hover:text-dark-green font-semibold underline break-all">
              tomaszeuskim@gmail.com
            </a>
          </p>
        </div>
      </div>
    </div>
  );
};
