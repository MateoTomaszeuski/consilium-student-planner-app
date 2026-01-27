namespace Consilium.Shared.Models;

public class TodoList {
    public TodoList() {
        Id = -1;
        ListName = "";
        TodoItems = new();
    }
    public TodoList(int id = -1, string listname = "", List<TodoItem>? todoItems = null) {
        Id = id;
        ListName = listname;
        TodoItems = todoItems is null ? new List<TodoItem>() : todoItems;
    }
    public int Id { get; set; }
    public string ListName { get; set; }
    public List<TodoItem> TodoItems { get; set; }
}