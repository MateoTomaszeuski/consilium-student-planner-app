using CommunityToolkit.Maui.Alerts;
using Consilium.Shared.ViewModels;
namespace Consilium.Maui.Views;
public partial class AssignmentsView : ContentPage {
    public AssignmentsView(AssignmentsViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
        Appearing += async (s, e) =>
        {
            await vm.InitializeViewModelAsync();
        };

        vm.ShowSnackbarAsync = async (message) =>
        {
            var snackbar = Snackbar.Make(message, duration: TimeSpan.FromSeconds(3));
            await snackbar.Show();
        };
    }
    private async void OnDeleteCourseClicked(object sender, EventArgs e) {
        bool confirmed = await DisplayAlert(
            "Delete Course?",
            "This will permanently delete the selected course and all of its assignments. Are you sure?",
            "OK",
            "Cancel");

        if (confirmed && BindingContext is AssignmentsViewModel vm) {
            await vm.DeleteCourseCommand.ExecuteAsync(true);
        }
    }
}