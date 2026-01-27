using Consilium.Shared.Services;
using Consilium.Shared.ViewModels;
using NSubstitute;

namespace Consilium.Tests;

public class ProfileVmTest {
    private ProfileViewModel viewModel;
    private ILogInService logInService;
    private IPersistenceService persistenceService;
    private bool snackbarCalled;
    private string lastSnackbarMessage = string.Empty;
    public ProfileVmTest() {
        logInService = Substitute.For<ILogInService>();
        persistenceService = Substitute.For<IPersistenceService>();
        viewModel = new ProfileViewModel(logInService, persistenceService) {
            ShowSnackbarAsync = async msg =>
            {
                snackbarCalled = true;
                lastSnackbarMessage = msg;
                await Task.CompletedTask;
            }
        };
        snackbarCalled = false;
        lastSnackbarMessage = string.Empty;
    }

    [Before(Test)]
    public void Setup() {
        logInService = Substitute.For<ILogInService>();
        persistenceService = Substitute.For<IPersistenceService>();
        viewModel = new ProfileViewModel(logInService, persistenceService) {
            ShowSnackbarAsync = async msg =>
            {
                snackbarCalled = true;
                lastSnackbarMessage = msg;
                await Task.CompletedTask;
            }
        };
        snackbarCalled = false;
        lastSnackbarMessage = string.Empty;
    }

    [Test]
    public async Task CanCreateViewModel() {
        await Assert.That(viewModel).IsNotNull();
    }

    [Test]
    public async Task InitialState_AllFlagsFalseOrEmpty() {
        await Assert.That(viewModel.LoggedIn).IsEqualTo(false);
        await Assert.That(viewModel.ShowLogIn).IsEqualTo(false);
        await Assert.That(viewModel.ShowUnAuthorized).IsEqualTo(false);
        await Assert.That(viewModel.ShowLogOut).IsEqualTo(false);
        await Assert.That(viewModel.EmailInput).IsEqualTo(string.Empty);
        await Assert.That(viewModel.Token).IsEqualTo(string.Empty);
        await Assert.That(viewModel.Username).IsEqualTo(string.Empty);
        await Assert.That(viewModel.Message).IsNull();
    }

    [Test]
    public async Task EmailIsValid_FalseForEmptyOrInvalid() {
        viewModel.EmailInput = "";
        await Assert.That(viewModel.EmailIsValid).IsEqualTo(false);

        viewModel.EmailInput = "not-an-email";
        await Assert.That(viewModel.EmailIsValid).IsEqualTo(false);
    }

    [Test]
    public async Task EmailIsValid_TrueForWellFormed() {
        viewModel.EmailInput = "user@example.com";
        await Assert.That(viewModel.EmailIsValid).IsEqualTo(true);
    }

    [Test]
    public async Task LogInCommand_DoesNothing_WhenEmailInvalid() {
        viewModel.EmailInput = "invalid";
        await viewModel.LogInCommand.ExecuteAsync(null);

        await logInService.DidNotReceive().LogIn(Arg.Any<string>());
        persistenceService.DidNotReceive().SaveToken(Arg.Any<string>(), Arg.Any<string>());
        await Assert.That(viewModel.LoggedIn).IsEqualTo(false);
        await Assert.That(snackbarCalled).IsEqualTo(false);
    }

    [Test]
    public async Task LogInCommand_SuccessfulAuth_SetsAllStateAndShowsSnackbar() {
        // Arrange
        viewModel.EmailInput = "test@domain.com";
        logInService.LogIn("test@domain.com").Returns(Task.FromResult("TOKEN123"));
        logInService.CheckAuthStatus().Returns(Task.FromResult(true));
        persistenceService.GetUserName().Returns("Tester");

        // Act
        await viewModel.LogInCommand.ExecuteAsync(null);

        // Assert service calls
        await logInService.Received(1).LogIn("test@domain.com");
        persistenceService.Received(1).SaveToken("test@domain.com", "TOKEN123");

        // Assert VM state
        await Assert.That(viewModel.LoggedIn).IsEqualTo(true);
        await Assert.That(viewModel.Username).IsEqualTo("Tester");
        await Assert.That(viewModel.ShowLogIn).IsEqualTo(false);
        await Assert.That(viewModel.ShowUnAuthorized).IsEqualTo(false);
        await Assert.That(viewModel.ShowLogOut).IsEqualTo(true);

        // Assert snackbar
        await Assert.That(snackbarCalled).IsEqualTo(true);
        await Assert.That(lastSnackbarMessage).IsEqualTo("Successfully logged in!");
    }

    [Test]
    public async Task LogInCommand_TooManyUnauthorized_ShowsErrorSnackbar() {
        viewModel.EmailInput = "user@domain.com";
        logInService.LogIn("user@domain.com").Returns(Task.FromResult("Too many unauthorized keys"));

        await viewModel.LogInCommand.ExecuteAsync(null);

        await Assert.That(viewModel.ShowUnAuthorized).IsEqualTo(false);
        await Assert.That(viewModel.ShowLogIn).IsEqualTo(true);
        await Assert.That(viewModel.ShowLogOut).IsEqualTo(false);
        await Assert.That(snackbarCalled).IsEqualTo(true);
        await Assert.That(lastSnackbarMessage).IsEqualTo("Too many unauthorized keys.");
    }

    [Test]
    public async Task LogOutCommand_ResetsStateAndShowsSnackbar() {
        // Arrange an already logged in state
        viewModel.LoggedIn = true;
        viewModel.Username = "Tester";
        viewModel.ShowLogOut = true;
        viewModel.ShowLogIn = false;

        // Act
        await viewModel.LogOutCommand.ExecuteAsync(null);

        // Assert service call
        await logInService.Received(1).LogOut();

        // Assert VM state
        await Assert.That(viewModel.LoggedIn).IsEqualTo(false);
        await Assert.That(viewModel.Username).IsEqualTo(string.Empty);
        await Assert.That(viewModel.ShowLogIn).IsEqualTo(true);
        await Assert.That(viewModel.ShowLogOut).IsEqualTo(false);
        await Assert.That(viewModel.ShowUnAuthorized).IsEqualTo(false);

        // Assert snackbar
        await Assert.That(snackbarCalled).IsEqualTo(true);
        await Assert.That(lastSnackbarMessage).IsEqualTo("Successfully logged out!");
    }

    [Test]
    public async Task SignOutAllDevicesCommand_ClearsUserAndCallsGlobalLogout() {
        viewModel.LoggedIn = true;
        viewModel.Username = "Tester";

        await viewModel.SignOutAllDevicesCommand.ExecuteAsync(null);

        await logInService.Received(1).GlobalLogOut();
        await Assert.That(viewModel.LoggedIn).IsEqualTo(false);
        await Assert.That(viewModel.Username).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task CheckUnAuthorizedCommand_TogglesFlagsAndShowsSnackbarWhenAuthorized() {
        viewModel.LoggedIn = true;
        logInService.CheckAuthStatus().Returns(Task.FromResult(true));

        await viewModel.CheckUnAuthorizedCommand.ExecuteAsync(null);

        await Assert.That(viewModel.ShowUnAuthorized).IsEqualTo(false);
        await Assert.That(viewModel.ShowLogOut).IsEqualTo(true);
        await Assert.That(snackbarCalled).IsEqualTo(true);
        await Assert.That(lastSnackbarMessage).IsEqualTo("Successfully logged in!");
    }

    [Test]
    public async Task InitializeAsync_SetsFlagsFromPersistenceAndAuthCheck() {
        persistenceService.CheckLocalLoginStatus().Returns(true);
        persistenceService.GetUserName().Returns("Tester");
        logInService.CheckAuthStatus().Returns(Task.FromResult(false));

        await viewModel.InitializeAsync();

        await Assert.That(viewModel.LoggedIn).IsEqualTo(true);
        await Assert.That(viewModel.ShowLogIn).IsEqualTo(false);
        await Assert.That(viewModel.ShowUnAuthorized).IsEqualTo(true);
        await Assert.That(viewModel.ShowLogOut).IsEqualTo(true);
        await Assert.That(viewModel.Username).IsEqualTo("Tester");
    }
}