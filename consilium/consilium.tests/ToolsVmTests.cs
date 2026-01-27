using Consilium.Shared.ViewModels;

namespace Consilium.Tests;

public class ToolsVmTest {
    private ToolsViewModel viewModel;

    public ToolsVmTest() {
        viewModel = new ToolsViewModel();
    }

    [Before(Test)]
    public void Setup() {
        viewModel = new ToolsViewModel();
    }

    [Test]
    public async Task CanCreateViewModel() {
        await Assert.That(viewModel).IsNotNull();
    }

    [Test]
    public async Task InitialState_AllInactive() {
        await Assert.That(viewModel.NotesActive).IsEqualTo(true);
        await Assert.That(viewModel.CalculatorActive).IsEqualTo(false);
        await Assert.That(viewModel.PomodoroActive).IsEqualTo(false);
        await Assert.That(viewModel.CalendarActive).IsEqualTo(false);
    }

    [Test]
    public async Task WhenNotesSelected_OnlyNotesActive() {
        viewModel.ChangeTool("Notes");
        await Assert.That(viewModel.NotesActive).IsEqualTo(true);
        await Assert.That(viewModel.CalculatorActive).IsEqualTo(false);
        await Assert.That(viewModel.PomodoroActive).IsEqualTo(false);
        await Assert.That(viewModel.CalendarActive).IsEqualTo(false);
    }

    [Test]
    public async Task WhenCalculatorSelected_OnlyCalculatorActive() {
        viewModel.ChangeTool("Calculator");
        await Assert.That(viewModel.NotesActive).IsEqualTo(false);
        await Assert.That(viewModel.CalculatorActive).IsEqualTo(true);
        await Assert.That(viewModel.PomodoroActive).IsEqualTo(false);
        await Assert.That(viewModel.CalendarActive).IsEqualTo(false);
    }

    [Test]
    public async Task WhenPomodoroSelected_OnlyPomodoroActive() {
        viewModel.ChangeTool("Pomodoro");
        await Assert.That(viewModel.NotesActive).IsEqualTo(false);
        await Assert.That(viewModel.CalculatorActive).IsEqualTo(false);
        await Assert.That(viewModel.PomodoroActive).IsEqualTo(true);
        await Assert.That(viewModel.CalendarActive).IsEqualTo(false);
    }

    [Test]
    public async Task WhenCalendarSelected_OnlyCalendarActive() {
        viewModel.ChangeTool("Calendar");
        await Assert.That(viewModel.NotesActive).IsEqualTo(false);
        await Assert.That(viewModel.CalculatorActive).IsEqualTo(false);
        await Assert.That(viewModel.PomodoroActive).IsEqualTo(false);
        await Assert.That(viewModel.CalendarActive).IsEqualTo(true);
    }

    [Test]
    public async Task WhenUnknownSelected_AllInactive() {
        viewModel.ChangeTool("Unknown");
        await Assert.That(viewModel.NotesActive).IsEqualTo(false);
        await Assert.That(viewModel.CalculatorActive).IsEqualTo(false);
        await Assert.That(viewModel.PomodoroActive).IsEqualTo(false);
        await Assert.That(viewModel.CalendarActive).IsEqualTo(false);
    }
}