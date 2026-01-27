using Consilium.Shared.ViewModels.Controls;
using System.ComponentModel;

namespace Consilium.Maui.Controls;

public partial class NotesView : ContentView {
    NotesViewModel _viewModel;
    public NotesView() {
        InitializeComponent();
        _viewModel = ((App)Application.Current).Services.GetService<NotesViewModel>();
        BindingContext = _viewModel;
        _viewModel.PropertyChanged += PropertyChanged_Handler;
    }

    private void PropertyChanged_Handler(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(_viewModel.Content)) {
            _viewModel.Dispose();
        }
    }
}