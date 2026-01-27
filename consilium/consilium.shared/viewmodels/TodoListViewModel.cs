using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Consilium.Shared.Models;
using Consilium.Shared.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Consilium.Shared.ViewModels;
public partial class TodoListViewModel : ObservableObject {
    private IToDoService todoService;
    public TodoListViewModel(IToDoService toDoService) {
        Categories = new ObservableCollection<string>() { "Misc.", "School", "Work" };
        FilterCategories = new ObservableCollection<string>(Categories.Append("All"));
        SelectedSortOption = SortOptions[0];
        NewTodoCategory = Categories[0];
        TodoItems = new();
        this.todoService = toDoService;
    }
    public async Task InitializeItemsAsync() {
        IsLoading = true;
        await todoService.InitializeTodosAsync();
        TodoItems = todoService.GetTodoItems();

        foreach (var item in TodoItems) {
            item.PropertyChanged += OnTodoItemPropertyChanged;
        }

        if (TodoItems.Count < 1) {
            Message = "No items found.";
        } else {
            Message = string.Empty;
        }
        IsLoading = false;

        AnyTasksAreCompleted = TodoItems.Any(item => item.IsCompleted);
    }

    public ObservableCollection<string> SortOptions { get; } = new() {
        "Category A-Z",
        "Category: Z-A",
        "Title: A-Z",
        "Title: Z-A",
        "Completion"
    };

    [ObservableProperty]
    private string selectedSortOption;

    [ObservableProperty]
    private ObservableCollection<string> categories;


    // appending an additional option so that they can go back to the unfiltered view
    [ObservableProperty]
    private ObservableCollection<string> filterCategories;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string newTodoTitle = "";

    [ObservableProperty]
    private ObservableCollection<TodoItem> todoItems;

    [ObservableProperty]
    private string newTodoCategory;

    [ObservableProperty]
    private string newSubtaskTitle = "";

    [ObservableProperty]
    private string message = "";

    [ObservableProperty]
    private string selectedCategory = "All";

    [ObservableProperty]
    private bool anyTasksAreCompleted = false;

    partial void OnSelectedCategoryChanged(string value) {
        IEnumerable<TodoItem> items;

        if (value == "All" || string.IsNullOrWhiteSpace(value)) {
            items = todoService.GetTodoItems();
        } else {
            items = todoService.GetTodosFilteredByCategory(value);
        }

        TodoItems = new ObservableCollection<TodoItem>(ApplySort(items));
    }

    partial void OnSelectedSortOptionChanged(string value) {
        if (TodoItems is null || TodoItems.Count < 1) return;

        IEnumerable<TodoItem> items;

        // maintain the filter while sorting
        if (SelectedCategory == "All" || string.IsNullOrWhiteSpace(SelectedCategory)) {
            items = todoService.GetTodoItems();
        } else {
            items = todoService.GetTodosFilteredByCategory(SelectedCategory);
        }

        TodoItems = new ObservableCollection<TodoItem>(ApplySort(items));
    }

    private void OnTodoItemPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(TodoItem.IsCompleted)) {
            UpdateAnyTasksAreCompleted();
        }
    }


    private void UpdateAnyTasksAreCompleted() {
        AnyTasksAreCompleted = TodoItems.Any(item => item.IsCompleted);
    }

    private IEnumerable<TodoItem> ApplySort(IEnumerable<TodoItem> items) {
        switch (SelectedSortOption) {
            case "Category: A-Z":
                return items.OrderBy(item => item.Category);
            case "Category: Z-A":
                return items.OrderByDescending(item => item.Category);
            case "Title: A-Z":
                return items.OrderBy(item => item.Title);
            case "Title: Z-A":
                return items.OrderByDescending(item => item.Title);
            case "Completion":
                return items.OrderBy(item => item.IsCompleted);
            default:
                return items;
        }
    }

    [RelayCommand]
    private async Task AddTodo() {
        if (!string.IsNullOrWhiteSpace(NewTodoTitle)) {
            var newTodo = new TodoItem(todoService) { Title = NewTodoTitle, Category = NewTodoCategory };
            await todoService.AddItemAsync(newTodo);
            newTodo.PropertyChanged += OnTodoItemPropertyChanged;
            TodoItems = todoService.GetTodoItems();

            // reapply the filter so users can see the list as they had it before
            OnSelectedCategoryChanged(SelectedCategory);
            NewTodoTitle = string.Empty;
            UpdateAnyTasksAreCompleted();
        }
    }

    [RelayCommand]
    private async Task RemoveTodo(TodoItem todoItem) {
        if (todoItem != null) {
            await todoService.RemoveToDoAsync(todoItem.Id);
            OnSelectedCategoryChanged(SelectedCategory);
            UpdateAnyTasksAreCompleted();
        }
    }


    [RelayCommand]
    private void ToggleSubtaskVisibility(TodoItem parentTask) {
        if (parentTask == null) return;

        if (parentTask.SubtaskEntryIsVisible)
            ToggleSubtaskEntryVisibility(parentTask);

        parentTask.IsExpanded = !parentTask.IsExpanded;
    }

    [RelayCommand]
    private void ToggleSubtaskEntryVisibility(TodoItem parentTask) {
        if (parentTask == null) return;

        foreach (var task in TodoItems) {
            if (task != parentTask && task.SubtaskEntryIsVisible) {
                task.SubtaskEntryIsVisible = false;
            }
        }

        parentTask.SubtaskEntryIsVisible = !parentTask.SubtaskEntryIsVisible;
        NewSubtaskTitle = "";
    }

    [RelayCommand]
    private async Task AddSubtask(TodoItem parentTask) {
        if (parentTask is null || string.IsNullOrWhiteSpace(NewSubtaskTitle) || SelectedCategory is null) return;
        await todoService.AddItemAsync(new TodoItem(todoService) { Title = NewSubtaskTitle, ParentId = parentTask.Id });

        TodoItems = todoService.GetTodoItems();

        parentTask.IsExpanded = true;
        NewSubtaskTitle = string.Empty;
    }

    [RelayCommand]
    private async Task RemoveSubtask(TodoItem subTask) {
        if (subTask?.ParentId is null) return;

        await todoService.RemoveToDoAsync(subTask.Id);
        OnSelectedCategoryChanged(SelectedCategory);
    }

    [RelayCommand]
    private async Task DeleteAllCompleted() {
        foreach (var item in TodoItems) {
            if (item.IsCompleted) {
                await todoService.RemoveToDoAsync(item.Id);
            }
        }

        // repopulates to-do items, while maintaining the selected sort/filter
        OnSelectedCategoryChanged(SelectedCategory);
        UpdateAnyTasksAreCompleted();
    }
}