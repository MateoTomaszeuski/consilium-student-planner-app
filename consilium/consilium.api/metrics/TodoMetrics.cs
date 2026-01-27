using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Consilium.API.Metrics;
public class TodoMetrics {
    private readonly UpDownCounter<int> _netTodosAdded;
    private readonly Histogram<double> _todoAddDuration;
    public TodoMetrics(IMeterFactory meterFactory) {
        var meter = meterFactory.Create("Consilium.Todos");
        _netTodosAdded = meter.CreateUpDownCounter<int>("consilium.todos.added");

        _todoAddDuration = meter.CreateHistogram<double>(
            name: "consilium.todos.add.duration",
            unit: "ms",
            description: "Time taken (in ms) to add a new to‑do");
    }

    public void TodoAdded() {
        _netTodosAdded.Add(1);
    }

    public void TodoRemoved() {
        _netTodosAdded.Add(-1);
    }

    public T TrackTodoAdd<T>(Func<T> addTodoFunc) {
        var sw = Stopwatch.StartNew();
        try {
            return addTodoFunc();
        } finally {
            sw.Stop();
            _todoAddDuration.Record(sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<T> TrackTodoAddAsync<T>(Func<Task<T>> addTodoFunc) {
        var sw = Stopwatch.StartNew();
        try {
            return await addTodoFunc();
        } finally {
            sw.Stop();
            _todoAddDuration.Record(sw.Elapsed.TotalMilliseconds);
        }
    }
}