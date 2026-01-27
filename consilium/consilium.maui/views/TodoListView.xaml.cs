using Consilium.Shared.Models;
using Consilium.Shared.ViewModels;
namespace Consilium.Maui.Views;
public partial class TodoListView : ContentPage {
    private TodoListViewModel vm;
    public TodoListView(TodoListViewModel vm) {
        InitializeComponent();
        this.vm = vm;
        BindingContext = vm;
        Appearing += async (s, e) =>
        {
            await vm.InitializeItemsAsync();
        };

    }
    private void NewTodoTitle_Completed(object sender, EventArgs e) {
        if (vm.AddTodoCommand.CanExecute(null) == true) {
            vm.AddTodoCommand.Execute(null);
        }
    }
    private void NewSubtaskTitle_Completed(object sender, EventArgs e) {
        var entry = sender as Entry;
        var parentTask = entry?.BindingContext as TodoItem;
        if (parentTask != null && vm.AddSubtaskCommand.CanExecute(parentTask)) {
            vm.AddSubtaskCommand.Execute(parentTask);
        }
    }

}