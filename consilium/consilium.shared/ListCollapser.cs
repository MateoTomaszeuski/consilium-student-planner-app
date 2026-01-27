using Consilium.Shared.Models;

namespace Consilium.Shared;

public static class ListCollapser {
    public static IEnumerable<TodoItem> CollapseList(IEnumerable<TodoItem> items) {
        List<TodoItem> newItems = new(items);
        for (int i = 0; i < newItems.Count; i++) {
            TodoItem item = newItems[i];
            if (item.ParentId is null) { continue; }

            TodoItem parent = items.First(a => a.Id == item.ParentId);
            if (!parent.Subtasks.Contains(item)) {
                parent.Subtasks.Add(item);
            }
            newItems.Remove(item);
            i--;
        }
        return newItems;
    }
}