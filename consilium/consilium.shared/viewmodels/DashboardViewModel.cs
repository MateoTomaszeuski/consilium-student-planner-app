using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Consilium.Shared.Models;
using Consilium.Shared.Services;
using System.Collections.ObjectModel;

namespace Consilium.Shared.ViewModels;

public partial class DashboardViewModel : ObservableObject {
    private readonly IPersistenceService persistenceService;
    private readonly ILogInService logInService;
    private readonly IToDoService toDoService;
    private readonly IAssignmentService assignmentService;
    [ObservableProperty]
    private string username = "Guest";
    [ObservableProperty]
    private string printMessage = String.Empty;
    [ObservableProperty]
    private ObservableCollection<Assignment> assignments = new();
    [ObservableProperty]
    private ObservableCollection<TodoItem> toDos = new();
    [ObservableProperty]
    private bool online = false;
    [ObservableProperty]
    private bool showDashboard = false;
    [ObservableProperty]
    private bool isLoading = false;
    public DashboardViewModel(IPersistenceService persistenceService, ILogInService logInService, IToDoService toDoService, IAssignmentService assignmentService) {

        this.persistenceService = persistenceService;
        this.logInService = logInService;
        this.toDoService = toDoService;
        this.assignmentService = assignmentService;
    }

    [RelayCommand]
    public async Task Initialize() {
        IsLoading = true;
        var u = persistenceService.GetUserName();
        u = u.Split('@')[0];
        Username = u != String.Empty ? u : "Guest";
        Online = await logInService.CheckAuthStatus();
        if (Username != "Guest" && Online) {
            PrintMessage = string.Empty;
            IEnumerable<Assignment> a = await assignmentService.GetAllAssignmentsAsync();
            Assignments = new(a.Take(3).OrderBy(a => a.DueDate));
            await toDoService.InitializeTodosAsync();
            ToDos = new(toDoService.GetTodoItems().Where(t => t.CompletionDate is null).OrderByDescending(t => t.Id).Take(5));
            ShowDashboard = true;
        } else {
            ShowDashboard = false;
            PrintMessage = "You are in Guest mode. To use all features, please make sure you're logged in and connected to the internet.";
        }
        IsLoading = false;
    }
}