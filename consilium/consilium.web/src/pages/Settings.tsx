import { useAppStore } from '../store/appStore';
import type { Theme } from '../types';

const THEMES: Theme[] = ['Green', 'Blue', 'Purple', 'Pink', 'Teal'];

export const Settings = () => {
  const { theme, setTheme } = useAppStore();

  return (
    <div className="settings">
      <h1>Settings</h1>

      <div className="settings-section">
        <h2>Theme</h2>
        <div className="theme-selector">
          {THEMES.map((t) => (
            <button
              key={t}
              className={`theme-button theme-${t.toLowerCase()} ${theme === t ? 'active' : ''}`}
              onClick={() => setTheme(t)}
            >
              {t}
            </button>
          ))}
        </div>
      </div>

      <div className="settings-section">
        <h2>Feedback</h2>
        <p>Send us your feedback to help improve Consilium</p>
        <textarea placeholder="Your feedback..." rows={5} />
        <button className="btn-primary">Send Feedback</button>
      </div>
    </div>
  );
};
