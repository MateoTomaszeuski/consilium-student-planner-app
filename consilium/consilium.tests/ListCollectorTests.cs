using Consilium.Shared;
using Consilium.Shared.Models;

namespace Consilium.Tests.ListCollector;

public class ListCollectorTests {
    [Test]
    public async Task CanReturnOneItem() {
        List<TodoItem> items = new() {
            new TodoItem() {Id = 1}
        };

        items = new(ListCollapser.CollapseList(items));

        await Assert.That(items[0].Id).IsEqualTo(1);
    }

    [Test]
    public async Task CanReturnTwoItems() {
        List<TodoItem> items = new() {
            new TodoItem() {Id = 1},
            new TodoItem() {Id = 2}
        };

        items = new(ListCollapser.CollapseList(items));

        await Assert.That(items[0].Id).IsEqualTo(1);
        await Assert.That(items[1].Id).IsEqualTo(2);
    }

    [Test]
    public async Task CanReturnTwoItemsAsASublist() {
        List<TodoItem> items = new() {
            new TodoItem() {Id = 1},
            new TodoItem() {Id = 2, ParentId = 1}
        };

        items = new(ListCollapser.CollapseList(items));

        await Assert.That(items.Count).IsEqualTo(1);
        await Assert.That(items[0].Subtasks.Count).IsEqualTo(1);
    }


    [Test]
    public async Task CanDoTheWholeThing() {
        List<TodoItem> items = new() {
            new TodoItem() {Id = 1},
            new TodoItem() {Id = 2, ParentId = 1},
            new TodoItem() {Id = 3},
            new TodoItem() {Id = 4, ParentId = 3},
            new TodoItem() {Id = 5, ParentId = 3},
            new TodoItem() {Id = 6},
            new TodoItem() {Id = 7, ParentId = 1},
            new TodoItem() {Id = 8, ParentId = 6},
            new TodoItem() {Id = 9},
            new TodoItem() {Id = 10}
        };

        items = new(ListCollapser.CollapseList(items));

        await Assert.That(items.Count).IsEqualTo(5);
        await Assert.That(items[0].Subtasks.Count).IsEqualTo(2);
        await Assert.That(items[1].Id).IsEqualTo(3);
        await Assert.That(items[1].Subtasks.Count).IsEqualTo(2);
        await Assert.That(items[2].Id).IsEqualTo(6);
        await Assert.That(items[2].Subtasks.Count).IsEqualTo(1);
    }
}