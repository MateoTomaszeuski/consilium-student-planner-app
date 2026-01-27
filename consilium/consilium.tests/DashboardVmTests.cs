using Consilium.Shared.Services;
using Consilium.Shared.ViewModels;
using NSubstitute;

namespace Consilium.Tests;

public class DashboardVmTests {
    private DashboardViewModel viewModel;
    private IPersistenceService persistenceService;
    private ILogInService logInService;
    private IToDoService toDoService;
    private IAssignmentService assignmentService;

    public DashboardVmTests() {
        persistenceService = Substitute.For<IPersistenceService>();
        logInService = Substitute.For<ILogInService>();
        toDoService = Substitute.For<IToDoService>();
        assignmentService = Substitute.For<IAssignmentService>();

        persistenceService.GetUserName().Returns("Guest");

        viewModel = new DashboardViewModel(
            persistenceService,
            logInService,
            toDoService,
            assignmentService
        );
    }
    [Before(Test)]
    public void Setup() {
        persistenceService = Substitute.For<IPersistenceService>();
        logInService = Substitute.For<ILogInService>();
        toDoService = Substitute.For<IToDoService>();
        assignmentService = Substitute.For<IAssignmentService>();

        // default to Guest
        persistenceService.GetUserName().Returns("Guest");

        viewModel = new DashboardViewModel(
            persistenceService,
            logInService,
            toDoService,
            assignmentService
        );
    }

    [Test]
    public async Task Constructor_EmptyUsername_SetsGuest() {
        persistenceService.GetUserName().Returns(string.Empty);

        await Assert.That(viewModel.Username).IsEqualTo("Guest");
    }

    [Test]
    public async Task Initialize_AsGuestOrOffline_ShowsGuestMessage() {
        persistenceService.GetUserName().Returns("Guest");
        logInService.CheckAuthStatus().Returns(Task.FromResult(false));

        await viewModel.InitializeCommand.ExecuteAsync(null);

        await Assert.That(viewModel.Online).IsEqualTo(false);
        await Assert.That(viewModel.ShowDashboard).IsEqualTo(false);
        await Assert.That(viewModel.PrintMessage).IsEqualTo(
            "You are in Guest mode. To use all features, please make sure you're logged in and connected to the internet."
        );
        await Assert.That(viewModel.Assignments).IsEmpty();
        await Assert.That(viewModel.ToDos).IsEmpty();
    }

}