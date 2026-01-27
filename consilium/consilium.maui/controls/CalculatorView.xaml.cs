using Consilium.Shared.ViewModels.Controls;

namespace Consilium.Maui.Controls;

public partial class CalculatorView : ContentView {
    public CalculatorView() {
        InitializeComponent();
        BindingContext = new CalculatorViewModel();
    }
}