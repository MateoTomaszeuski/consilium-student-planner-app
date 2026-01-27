using Consilium.Shared.ViewModels;
namespace Consilium.Maui.Views;
public partial class DashboardView : ContentPage {
    public DashboardView(DashboardViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
        Appearing += async (s, o) =>
        {
            await vm.Initialize();
        };

    }
}