using Consilium.Shared.Services;

namespace Consilium.Maui {
    public partial class App : Application {
        public App(IServiceProvider services) {
            InitializeComponent();
            InitializeComponent();
            Services = services;
            var persistence = services.GetService<IPersistenceService>();
            var savedTheme = persistence?.GetTheme() ?? "GreenTheme";
            ThemeManager.ApplyTheme(savedTheme);
        }

        public IServiceProvider Services { get; }

        protected override Window CreateWindow(IActivationState? activationState) {
            return new Window(new AppShell());
        }
    }
}