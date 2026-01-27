using Consilium.Shared.Models;
using Consilium.Shared.Services;
using Consilium.Shared.ViewModels;
using NSubstitute;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
namespace Consilium.Tests;


public class ToDoListVMTests {
    private readonly TodoListViewModel viewModel;
    private readonly ToDoService service;


    public ToDoListVMTests() {
        IHttpClientFactory factory = Substitute.For<IHttpClientFactory>();

        // Configure client within factory
        HttpClient client = Substitute.For<HttpClient>();
        client.PostAsJsonAsync("/post", new { }).ReturnsForAnyArgs(new HttpResponseMessage(System.Net.HttpStatusCode.Created) { Content = new StringContent("1") });
        factory.CreateClient().ReturnsForAnyArgs(client);


        IPersistenceService service = Substitute.For<IPersistenceService>();
        IClientService clientService = Substitute.For<ClientService>(factory);
        ILogInService logInService = Substitute.For<ILogInService>();
        ToDoService s = new ToDoService(service, clientService, logInService, true);
        this.service = s;

        viewModel = new TodoListViewModel(s);
    }

    [Test]
    public async Task CanCreateViewModel() {
        await Assert.That(viewModel).IsNotNull();
    }

    [Test]
    public async Task CanAddTodo() {
        viewModel.NewTodoTitle = "Test Todo";
        viewModel.AddTodoCommand.Execute(null);
        await Assert.That(viewModel.TodoItems.Count).IsEqualTo(1);
    }

    [Test]
    public async Task CanRemoveTodo() {
        viewModel.NewTodoTitle = "Test Todo";
        viewModel.AddTodoCommand.Execute(null);
        viewModel.RemoveTodoCommand.Execute(viewModel.TodoItems[0]);
        await Assert.That(viewModel.TodoItems.Count).IsEqualTo(0);
    }

    [Test]
    public async Task CantAddEmptyTodo() {
        viewModel.NewTodoTitle = "";
        viewModel.AddTodoCommand.Execute(null);
        await Assert.That(viewModel.TodoItems.Count).IsEqualTo(0);
    }

    [Test]
    public async Task CheckCorrectTodoItemIsAdded() {
        viewModel.NewTodoTitle = "Test Todo";
        viewModel.AddTodoCommand.Execute(null);
        await Assert.That(viewModel.TodoItems[0].Title).IsEqualTo("Test Todo");
    }

    [Test]
    public async Task CanAddMultipleItems() {
        viewModel.NewTodoTitle = "Test Todo";
        viewModel.AddTodoCommand.Execute(null);
        viewModel.NewTodoTitle = "Test Todo 2";
        viewModel.AddTodoCommand.Execute(null);
        await Assert.That(viewModel.TodoItems.Count).IsEqualTo(2);
    }

    [Test]
    public async Task GetTodoItemsList() {
        viewModel.NewTodoTitle = "Test Todo";
        viewModel.AddTodoCommand.Execute(null);
        viewModel.NewTodoTitle = "Test Todo 2";
        viewModel.AddTodoCommand.Execute(null);

        List<TodoItem> TodoItems = new List<TodoItem>(viewModel.TodoItems);
        await Assert.That(TodoItems[0].Title).IsEqualTo("Test Todo");

        await Assert.That(TodoItems[1].Title).IsEqualTo("Test Todo 2");

    }

    [Test]
    public async Task CanDeleteItem() {
        viewModel.NewTodoTitle = "Test Todo";
        viewModel.AddTodoCommand.Execute(null);
        viewModel.NewTodoTitle = "Test Todo 2";
        viewModel.AddTodoCommand.Execute(null);
        viewModel.RemoveTodoCommand.Execute(viewModel.TodoItems[0]);
        List<TodoItem> TodoItems = new List<TodoItem>(viewModel.TodoItems);
        await Assert.That(TodoItems.Count).IsEqualTo(1);
        await Assert.That(TodoItems[0].Title).IsEqualTo("Test Todo 2");
    }

    [Test]
    public async Task CanAddSingleSubtask() {
        // Direct access to the service, because it's needed
        service.TodoItems = new List<TodoItem>() {
            new TodoItem() { Title = "Task 1", Id = 1 },
            new TodoItem() { Title = "Task 2", Id = 2 }
        };

        // Just had to be done, I guess
        viewModel.TodoItems = new ObservableCollection<TodoItem>(service.TodoItems);

        viewModel.NewSubtaskTitle = "Subtask 1";
        viewModel.AddSubtaskCommand.Execute(viewModel.TodoItems[0]);

        // check that TodoItem with ID1 has a single subtask
        await Assert.That(viewModel.TodoItems[0].Subtasks.Count).IsEqualTo(1);
    }


    [Test]
    public async Task WhenIsCompletedBecomesTrue_CompletionDateIsSet() {
        viewModel.TodoItems = new ObservableCollection<TodoItem>() {
            new TodoItem() { Title = "Task 1", Id = 1 },
            new TodoItem() { Title = "Task 2", Id = 2 }
        };

        await Assert.That(viewModel.TodoItems[0].CompletionDate).IsNull();

        viewModel.TodoItems[0].IsCompleted = true;
        await Assert.That(viewModel.TodoItems[0].CompletionDate).IsNotNull();
    }

    [Test]
    public async Task WhenIsCompletedIsFalse_CompletionDateIsNull() {
        viewModel.TodoItems = new ObservableCollection<TodoItem>() {
            new TodoItem() { Title = "Task 1", Id = 1 },
            new TodoItem() { Title = "Task 2", Id = 2 }
        };

        await Assert.That(viewModel.TodoItems[0].CompletionDate).IsNull();

        viewModel.TodoItems[0].IsCompleted = true;
        await Assert.That(viewModel.TodoItems[0].CompletionDate).IsNotNull();

        viewModel.TodoItems[0].IsCompleted = false;
        await Assert.That(viewModel.TodoItems[0].CompletionDate).IsNull();
    }

    [Test]
    public async Task CanSortByAscendingCategory() {
        var testItems = new List<TodoItem>() {
            new TodoItem() { Title = "Task 1", Id = 1, Category="Banana" },
            new TodoItem() { Title = "Task 2", Id = 2, Category="Apple" }
        };

        service.TodoItems = testItems;
        viewModel.SelectedSortOption = "Category: Z-A";
        viewModel.TodoItems = new ObservableCollection<TodoItem>(service.TodoItems);

        viewModel.SelectedSortOption = "Category: A-Z";

        await Assert.That(viewModel.TodoItems[0].Category).IsEqualTo("Apple");
        await Assert.That(viewModel.TodoItems[1].Category).IsEqualTo("Banana");
    }

    [Test]
    public async Task AscendingSortWorksForThreeItemsWithDistinctCategories() {
        var testItems = new List<TodoItem> {
            new() { Title = "Task 1", Id = 1, Category="Banana" },
            new() { Title = "Task 2", Id = 2, Category="Cookie" },
            new() { Title = "Task 3", Id = 3, Category="Apple" }
        };


        service.TodoItems = testItems;
        viewModel.SelectedSortOption = "Category: Z-A";
        viewModel.TodoItems = new ObservableCollection<TodoItem>(service.TodoItems);

        viewModel.SelectedSortOption = "Category: A-Z";

        await Assert.That(viewModel.TodoItems[0].Category).IsEqualTo("Apple");
        await Assert.That(viewModel.TodoItems[1].Category).IsEqualTo("Banana");
        await Assert.That(viewModel.TodoItems[2].Category).IsEqualTo("Cookie");
    }


    [Test]
    public async Task CanFilterToDosByCategory() {
        var testItems = new List<TodoItem> {
            new() { Title = "Task 1", Id = 1, Category="School" },
            new() { Title = "Task 2", Id = 2, Category="Work" },
            new() { Title = "Task 3", Id = 3, Category="Work" },
            new() { Title = "Task 4", Id = 4, Category="School" }
        };

        service.TodoItems = testItems;
        viewModel.SelectedCategory = "All";
        viewModel.TodoItems = new ObservableCollection<TodoItem>(service.TodoItems);
        viewModel.SelectedCategory = "School";

        await Assert.That(viewModel.TodoItems.Count).IsEqualTo(2);
        await Assert.That(viewModel.TodoItems[0].Category).IsEqualTo("School");
        await Assert.That(viewModel.TodoItems[1].Category).IsEqualTo("School");
    }
}