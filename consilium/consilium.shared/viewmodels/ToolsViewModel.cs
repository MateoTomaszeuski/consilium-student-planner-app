using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Consilium.Shared.ViewModels;

public partial class ToolsViewModel : ObservableObject {
    [ObservableProperty]
    private bool notesActive = true;
    [ObservableProperty]
    private bool calculatorActive = false;
    [ObservableProperty]
    private bool pomodoroActive = false;
    [ObservableProperty]
    private bool calendarActive = false;

    [RelayCommand]
    public void ChangeTool(string tool) {
        NotesActive = "Notes" == tool;
        CalculatorActive = "Calculator" == tool;
        PomodoroActive = "Pomodoro" == tool;
        CalendarActive = "Calendar" == tool;
    }
}