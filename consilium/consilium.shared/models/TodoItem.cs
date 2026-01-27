using CommunityToolkit.Mvvm.ComponentModel;
using Consilium.Shared.Services;
using System.Collections.ObjectModel;

namespace Consilium.Shared.Models;

public partial class TodoItem : ObservableObject {
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string? Title { get; set; }
    public int TodoListId { get; set; }
    public int? AssignmentId { get; set; }
    public DateTime? CompletionDate { get; set; }

    private IToDoService? todoService;

    public TodoItem() {
        // need this for deserialization
    }

    public TodoItem(IToDoService service) {
        todoService = service;
    }

    public void InjectService(IToDoService service) {
        todoService = service;
    }

    [ObservableProperty]
    private string? category = String.Empty;

    public bool HasSubtasks { get => Subtasks.Count > 0; }

    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private bool subtaskEntryIsVisible;

    [ObservableProperty]
    private bool isCompleted;

    public void InitializeCompletionStatus() {
        IsCompleted = CompletionDate.HasValue;
    }

    partial void OnIsCompletedChanged(bool value) {
        if (value) {
            CompletionDate = DateTime.Now;
        } else {
            CompletionDate = null;
        }

        _ = SaveCompletionAsync();

        // tell the parent viewmodel that the completion status has changed
        OnPropertyChanged(nameof(IsCompleted));
    }

    private async Task SaveCompletionAsync() {
        if (todoService == null) {
            return;
        }
        await todoService.UpdateItemAsync(this);
    }


    public ObservableCollection<TodoItem> Subtasks { get; set; } = new();

    public bool Equals(TodoItem? other) {
        if (other == null) return false;

        return Id == other.Id;
    }

    public override string ToString() {
        string parentId;
        if (ParentId is null) {
            parentId = "null";
        } else {
            parentId = ParentId.ToString()!;
        }
        return $"Id: {Id}, Parent: {parentId}, Title: {Title}";
    }
}