using CommunityToolkit.Maui.Views;
using Consilium.Maui.PopUps;
using Consilium.Shared.ViewModels;

namespace Consilium.Maui.Views;

public partial class StatsView : ContentPage {
    public StatsView(StatsViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }

    private void Button_Clicked(object sender, EventArgs e) {
        this.ShowPopup(new NewFeatureSent());
    }
}