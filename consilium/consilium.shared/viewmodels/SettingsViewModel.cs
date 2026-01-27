using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Consilium.Shared.Services;
using System.Collections.ObjectModel;

namespace Consilium.Shared.ViewModels;
public partial class SettingsViewModel : ObservableObject {
    private readonly IPersistenceService persistence;
    private readonly IClientService client;

    public SettingsViewModel(IPersistenceService persistence, IClientService client) {
        this.persistence = persistence;
        this.client = client;
        Themes = new ObservableCollection<string> {
            "Green", "Blue", "Purple", "Pink","Teal" };
        SelectedTheme = Themes.FirstOrDefault(t => t == persistence.GetTheme()) ?? "Green";
    }

    [ObservableProperty]
    private ObservableCollection<string> themes;

    [ObservableProperty]
    private string selectedTheme;

    partial void OnSelectedThemeChanged(string value) {
        if (!string.IsNullOrEmpty(value)) {
            persistence.SaveTheme(value);
        }
    }

    [ObservableProperty]
    private bool sendFeedBackBool = false;
    [ObservableProperty]
    private string content = string.Empty;

    [RelayCommand]
    private void ActivateFeedBack() {
        SendFeedBackBool = true;
    }
    [RelayCommand]
    private async Task SendFeedBack() {
        var r = await client.GetAsync($"/NewFeature/feedback/{Content}");
        if (r.IsSuccessStatusCode) {
            SendFeedBackBool = false;
            Content = string.Empty;
        } else {
            // Handle error
        }
        SendFeedBackBool = false;
        Content = string.Empty;
    }
    [RelayCommand]
    private void CancelFeedback() {
        Content = string.Empty;
        SendFeedBackBool = false;
    }
}