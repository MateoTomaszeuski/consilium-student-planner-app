using CommunityToolkit.Mvvm.Messaging;
using Consilium.Shared.ViewModels;

namespace Consilium.Maui.Views;

public partial class SettingsView : ContentPage {
    private SettingsViewModel vm;

    public SettingsView() {
        InitializeComponent();
        vm = ((App)Application.Current).Services.GetService<SettingsViewModel>();
        BindingContext = vm;

        vm.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(SettingsViewModel.SelectedTheme)) {
                ApplyAndBroadcastTheme(vm.SelectedTheme);
            }
        };

        ThemePicker.SelectedIndexChanged += (s, e) =>
        {
            var selected = ThemePicker.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selected)) {
                ThemeManager.ApplyTheme(selected);
                Preferences.Set("SelectedTheme", selected);
            }
        };


    }

    protected override void OnAppearing() {
        base.OnAppearing();
        ThemeManager.ApplyTheme(vm.SelectedTheme);
    }

    private void ApplyAndBroadcastTheme(string themeName) {
        ThemeManager.ApplyTheme(themeName);
        Preferences.Set("SelectedTheme", themeName);

        // tell android to update status bar color
        WeakReferenceMessenger.Default.Send(new ThemeChangedMessage(themeName));
    }
}