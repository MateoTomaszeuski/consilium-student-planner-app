using Consilium.Shared.ViewModels.Controls;
using Plugin.Maui.Audio;

namespace Consilium.Maui.Controls;

public partial class PomodoroView : ContentView {
    private readonly IAudioManager? _audioManager;

    public PomodoroView() {
        InitializeComponent();
        _audioManager = ServiceHelper.GetService<IAudioManager>();
        var vm = new PomodoroViewModel();
        BindingContext = vm;
        vm.PropertyChanged += (s, o) =>
        {
            if (o.PropertyName == nameof(PomodoroViewModel.CurrentAction)) {
                PlayChangeSong(this, EventArgs.Empty);
            }
        };

    }

    public PomodoroView(IAudioManager audioManager) : this() {
        _audioManager = audioManager;
    }

    private async void PlayChangeSong(object sender, EventArgs e) {
        if (_audioManager is null) return;

        using var stream = await FileSystem.OpenAppPackageFileAsync("timer.mp3");
        var player = _audioManager.CreatePlayer(stream);
        player.Volume = 1.0;
        player.Play();
    }
    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e) {
        var slider = (Slider)sender;
        int step = 1;
        int rounded = (int)Math.Round(e.NewValue / step) * step;

        if (rounded != (int)e.NewValue)
            slider.Value = rounded;

        if (BindingContext is PomodoroViewModel vm)
            vm.WorkTime = rounded;
    }
}