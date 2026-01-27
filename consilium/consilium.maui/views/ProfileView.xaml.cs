using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Consilium.Maui.PopUps;
using Consilium.Shared.Services;
using Consilium.Shared.ViewModels;


namespace Consilium.Maui.Views;

public partial class ProfileView : ContentPage {
    private ProfileViewModel vm;

    public ProfileView(ProfileViewModel vm) {
        this.vm = vm;
        InitializeComponent();
        BindingContext = vm;

        Appearing += async (s, e) =>
        {
            await vm.InitializeAsync();
        };
        WeakReferenceMessenger.Default.Register<ShowPopupMessage>(this, (r, m) =>
        {
            this.ShowPopup(new LoggedInPopUp());
        });

        vm.ShowSnackbarAsync = async (message) =>
        {
            var snackbar = Snackbar.Make(message, duration: TimeSpan.FromSeconds(3));
            await snackbar.Show();
        };

    }

    private void EmailEntry_Completed(object sender, EventArgs e) {
        if (vm.LogInCommand.CanExecute(null) == true) {
            vm.LogInCommand.Execute(null);
        }
    }
}