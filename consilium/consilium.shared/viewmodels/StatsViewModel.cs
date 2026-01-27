using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Consilium.Shared.Services;

namespace Consilium.Shared.ViewModels;
public partial class StatsViewModel(IClientService client) : ObservableObject {
    [ObservableProperty]
    private bool sent = false;
    [RelayCommand]
    public async Task SendStats() {
        await client.GetAsync("/newfeature");
        Sent = true;
    }
}