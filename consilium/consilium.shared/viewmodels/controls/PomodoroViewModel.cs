using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Consilium.Shared.ViewModels.Controls;
public partial class PomodoroViewModel : ObservableObject {
    private Timer? _timer;
    private int _currentTimer;

    [ObservableProperty]
    private int workTime = 20; // Default 20 minutes

    [ObservableProperty]
    private int breakTime = 5; // Default 5 minutes

    [ObservableProperty]
    private string currentAction = "Working";

    [ObservableProperty]
    private bool isTimerRunning = false;

    public PomodoroViewModel() {
        _currentTimer = WorkTime * 60;
    }

    [RelayCommand]
    public void StartTimer() {
        if (IsTimerRunning) return;

        IsTimerRunning = true;
        _timer = new Timer(TimerCallback, null, 0, 1000);
    }

    [RelayCommand]
    public void StopTimer() {
        _timer?.Dispose();
        _timer = null;
        IsTimerRunning = false;
    }

    [RelayCommand]
    public void ResetTimer() {
        StopTimer();
        CurrentAction = "Working";
        _currentTimer = WorkTime * 60;
        OnPropertyChanged(nameof(CurrentTimerDisplay));
    }

    private void TimerCallback(object? state) {
        if (_currentTimer > 0) {
            _currentTimer--;
            OnPropertyChanged(nameof(CurrentTimerDisplay));
        } else {
            SwitchAction();
        }
    }

    private void SwitchAction() {
        if (CurrentAction == "Working") {
            CurrentAction = "Break";
            _currentTimer = BreakTime * 60;
            // Make sound
        } else {
            CurrentAction = "Working";
            _currentTimer = WorkTime * 60;
            // Make sound
        }
        OnPropertyChanged(nameof(CurrentTimerDisplay));
    }
    partial void OnWorkTimeChanged(int value) {
        if (!IsTimerRunning && CurrentAction == "Working") {
            _currentTimer = value * 60;
            OnPropertyChanged(nameof(CurrentTimerDisplay));
        }
    }
    partial void OnBreakTimeChanged(int value) {
        if (!IsTimerRunning && CurrentAction == "Break") {
            _currentTimer = value * 60;
            OnPropertyChanged(nameof(CurrentTimerDisplay));
        }
    }
    public string CurrentTimerDisplay =>
        TimeSpan.FromSeconds(_currentTimer)
               .ToString(@"mm\:ss");
}