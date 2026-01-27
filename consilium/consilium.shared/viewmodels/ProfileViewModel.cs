using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Consilium.Shared.Services;

namespace Consilium.Shared.ViewModels;
public partial class ProfileViewModel : ObservableObject {
    public ProfileViewModel(ILogInService logInService, IPersistenceService persistenceService) {
        this.logInService = logInService;
        this.persistenceService = persistenceService;
    }

    [ObservableProperty]
    private bool loggedIn = false;
    [ObservableProperty]
    private bool showLogIn;
    [ObservableProperty]
    private bool showUnAuthorized = false;
    [ObservableProperty]
    private bool showLogOut = false;
    [ObservableProperty]
    private string emailInput = String.Empty;
    [ObservableProperty]
    private string token = String.Empty;
    [ObservableProperty]
    private string? message;
    [ObservableProperty]
    private string username = string.Empty;
    private readonly ILogInService logInService;
    private readonly IPersistenceService persistenceService;

    public bool EmailIsValid => IsValidEmail(EmailInput);

    partial void OnEmailInputChanged(string value) {
        OnPropertyChanged(nameof(EmailIsValid));
    }


    public Func<string, Task>? ShowSnackbarAsync { get; set; }


    [RelayCommand]
    private async Task LogIn() {
        if (string.IsNullOrEmpty(EmailInput) || !IsValidEmail(EmailInput)) return;

        Token = await logInService.LogIn(EmailInput);
        persistenceService.SaveToken(EmailInput, Token);

        if (Token != "Too many unauthorized keys") {
            ShowLoggedInPopup();
            LoggedIn = true;
            Username = persistenceService.GetUserName();
            ShowLogIn = false;

            bool isValidated = await logInService.CheckAuthStatus();
            ShowUnAuthorized = !isValidated;
            ShowLogOut = LoggedIn;

            if (!isValidated) {
                await PollForAuthorizationAsync();
            } else {
                if (ShowSnackbarAsync is not null)
                    await ShowSnackbarAsync("Successfully logged in!");
            }
        } else {
            ShowUnAuthorized = false;
            ShowLogIn = true;
            ShowLogOut = false;
            if (ShowSnackbarAsync is not null)
                await ShowSnackbarAsync("Too many unauthorized keys.");
        }
    }

    [RelayCommand]
    private async Task LogOut() {
        // cancel any previous polling
        _authPollingCts?.Cancel();

        await logInService.LogOut();
        LoggedIn = false;
        Username = string.Empty;
        ShowLogIn = true;
        ShowLogOut = false;
        ShowUnAuthorized = false;
        if (ShowSnackbarAsync is not null)
            await ShowSnackbarAsync("Successfully logged out!");
    }

    [RelayCommand]
    private async Task SignOutAllDevices() {
        await logInService.GlobalLogOut();
        Username = string.Empty;
        LoggedIn = false;
    }

    [RelayCommand]
    private async Task CheckUnAuthorized() {
        ShowUnAuthorized = !await logInService.CheckAuthStatus() && LoggedIn;
        ShowLogOut = LoggedIn;

        // show snackbar notification once the user has successfully verified account
        if (LoggedIn) {
            if (ShowSnackbarAsync is not null)
                await ShowSnackbarAsync("Successfully logged in!");
        }
    }
    [RelayCommand]
    private void ShowLoggedInPopup() {
        WeakReferenceMessenger.Default.Send(new ShowPopupMessage());
    }
    public async Task InitializeAsync() {
        LoggedIn = persistenceService.CheckLocalLoginStatus();
        ShowUnAuthorized = !await logInService.CheckAuthStatus() && LoggedIn;
        ShowLogIn = !LoggedIn;
        Username = persistenceService.GetUserName();
        ShowLogOut = LoggedIn;
    }

    private bool IsValidEmail(string email) {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try {
            return System.Text.RegularExpressions.Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        } catch {
            return false;
        }
    }


    private CancellationTokenSource? _authPollingCts;


    // polling so the user doesn't have to manually refresh after validating - thanks ChatGPT...
    private async Task PollForAuthorizationAsync() {
        _authPollingCts?.Cancel(); // cancel any previous polling
        _authPollingCts = new CancellationTokenSource();

        // times out after 90 seconds
        var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_authPollingCts.Token, timeoutCts.Token);
        var token = linkedCts.Token;

        try {
            while (!token.IsCancellationRequested && ShowUnAuthorized) {
                await Task.Delay(TimeSpan.FromSeconds(5), token);

                bool isAuthorized = await logInService.CheckAuthStatus();

                if (isAuthorized) {
                    ShowUnAuthorized = false;
                    if (ShowSnackbarAsync is not null)
                        await ShowSnackbarAsync("Successfully logged in!");

                    break; // stop polling
                }
            }
        } catch (TaskCanceledException) {
            if (timeoutCts.IsCancellationRequested && ShowUnAuthorized) {
                if (ShowSnackbarAsync is not null)
                    await ShowSnackbarAsync("Still waiting for validation. Please refresh manually after validating your email.");
            }
        }
    }

}