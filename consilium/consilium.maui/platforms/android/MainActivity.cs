using Android.App;
using Android.Content.PM;
using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;

using MauiApp = Microsoft.Maui.Controls.Application;
using MauiColor = Microsoft.Maui.Graphics.Color;

namespace Consilium.Maui;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize |
                           ConfigChanges.Orientation |
                           ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout |
                           ConfigChanges.SmallestScreenSize |
                           ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity {
    protected override void OnCreate(Bundle? savedInstanceState) {
        base.OnCreate(savedInstanceState);

        UpdateStatusBarColor();

        WeakReferenceMessenger.Default.Register<ThemeChangedMessage>(this, (r, message) =>
        {
            UpdateStatusBarColor();
        });
    }

    private void UpdateStatusBarColor() {
        try {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop &&
                MauiApp.Current?.Resources.TryGetValue("Primary", out var colorObj) == true &&
                colorObj is MauiColor mauiColor) {
                var androidColor = mauiColor.ToPlatformColor();
                Window?.SetStatusBarColor(androidColor);
            }
        } catch {
            // ignore or log
        }
    }
}

public static class ColorExtensions {
    public static Android.Graphics.Color ToPlatformColor(this MauiColor color) {
        return Android.Graphics.Color.Argb(
            (int)(color.Alpha * 255),
            (int)(color.Red * 255),
            (int)(color.Green * 255),
            (int)(color.Blue * 255)
        );
    }
}