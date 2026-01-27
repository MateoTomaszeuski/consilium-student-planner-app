using CommunityToolkit.Maui;
using Consilium.Maui.Controls;
using Consilium.Maui.Views;
using Consilium.Shared.Models;
using Consilium.Shared.Services;
using Consilium.Shared.ViewModels;
using Consilium.Shared.ViewModels.Controls;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace Consilium.Maui;

public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
           .UseMauiApp<App>()
           .UseMauiCommunityToolkit(options =>
               {
                   options.SetShouldEnableSnackbarOnWindows(true);
               })
            .RegisterViews()
            .RegisterViewModels()
            .RegisterServices()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Inter.ttf", "Inter");
                fonts.AddFont("fa-free-regular.otf", "FontAwesomeRegular");
                fonts.AddFont("fa-free-solid.otf", "FontAwesomeSolid");
            });



#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("https://consilium-api-cpgdcqaxepbyc2gj.westus3-01.azurewebsites.net/");
            //client.BaseAddress = new Uri("https://main.consilium.duckdns.org/"); // If you want to hit kubernetees, make sure to be in the vpn

#if DEBUG
            //client.BaseAddress = new Uri("http://localhost:5202");
#endif
        });


        var app = builder.Build();

        using (var scope = app.Services.CreateScope()) {
            var service = scope.ServiceProvider.GetRequiredService<IPersistenceService>();
            service.OnStartup();
        }

        return app;

    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder) {
        builder.Services.AddSingleton<AssignmentsView>();
        builder.Services.AddSingleton<ChatView>();
        builder.Services.AddSingleton<DashboardView>();
        builder.Services.AddSingleton<ProfileView>();
        builder.Services.AddSingleton<TodoListView>();
        builder.Services.AddSingleton<ToolsView>();
        builder.Services.AddSingleton<CalculatorView>();
        builder.Services.AddSingleton<Calendar>();
        builder.Services.AddSingleton<MessagesView>();
        builder.Services.AddSingleton<NotesView>();
        builder.Services.AddSingleton<PomodoroView>();
        builder.Services.AddSingleton<SettingsView>();
        builder.Services.AddSingleton<StatsView>();

        return builder;
    }


    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder) {
        builder.Services.AddSingleton<AssignmentsViewModel>();
        builder.Services.AddSingleton<ChatViewModel>();
        builder.Services.AddSingleton<DashboardViewModel>();
        builder.Services.AddSingleton<ProfileViewModel>();
        builder.Services.AddSingleton<TodoListViewModel>();
        builder.Services.AddSingleton<ToolsViewModel>();
        builder.Services.AddSingleton<CalculatorViewModel>();
        builder.Services.AddSingleton<MessagesViewModel>();
        builder.Services.AddSingleton<NotesViewModel>();
        builder.Services.AddSingleton<PomodoroViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<StatsViewModel>();
        return builder;
    }
    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder) {
        builder.Services.AddSingleton<IToDoService, ToDoService>();
        builder.Services.AddSingleton<IPersistenceService, PersistenceService>();
        builder.Services.AddSingleton<ILogInService, LogInService>();
        builder.Services.AddSingleton<IClientService, ClientService>();
        builder.Services.AddSingleton<IMessageService, MessageService>();
        builder.Services.AddSingleton<IAssignmentService, AssignmentService>();
        builder.Services.AddSingleton(AudioManager.Current);

        return builder;
    }
}