import { useAppStore } from '../store/appStore';
import { authService } from '../services/authService';
import type { Theme } from '../types';

const THEMES: Theme[] = ['Green', 'Blue', 'Purple', 'Pink', 'Teal'];

export const Settings = () => {
  const { theme, setTheme } = useAppStore();

  if (!authService.isLoggedIn()) {
    return (
      <div className="space-y-8">
        <h1 className="text-3xl font-bold text-center text-dark-dark">Settings</h1>
        <div className="bg-light-med border border-dark-med/30 rounded-lg p-6 text-center max-w-3xl mx-auto">
          <p className="text-dark-dark">
            You are in Guest mode. To use all features, please go to the Profile page and log in.
          </p>
        </div>
      </div>
    );
  }

  const handleThemeChange = async (newTheme: Theme) => {
    setTheme(newTheme);
    await authService.updateTheme(newTheme);
  };

  return (
    <div className="max-w-4xl mx-auto">
      <h1 className="text-3xl font-bold mb-8 text-dark-dark">Settings</h1>

      <div className="space-y-6">
        <div className="bg-light-light border border-dark-med/20 rounded-xl p-6 shadow-sm">
          <h2 className="text-2xl font-bold mb-4 text-dark-dark">Theme</h2>
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-5 gap-3">
            {THEMES.map((t) => (
              <button
                key={t}
                className={`px-4 py-3 rounded-lg font-semibold transition-all border-2 ${
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

        <div className="bg-light-light border border-dark-med/20 rounded-xl p-6 shadow-sm">
          <h2 className="text-2xl font-bold mb-4 text-dark-dark">Support</h2>
          <p className="text-dark-med">
            For recommendations, feedback, or support, please contact{' '}
            <a href="mailto:tomaszeuskim@gmail.com" className="text-mid-green hover:text-dark-green font-semibold underline">
              tomaszeuskim@gmail.com
            </a>
          </p>
        </div>
      </div>
    </div>
  );
};
