using CommunityToolkit.Maui;
using EventHubApp.Data;
using EventHubApp.Services;
using EventHubApp.ViewModels;
using EventHubApp.Views;
using Microsoft.Extensions.Logging;

namespace EventHubApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<AppDatabase>();
            builder.Services.AddSingleton<SessionService>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<EventsViewModel>();
            builder.Services.AddTransient<ManageEventViewModel>();
            builder.Services.AddTransient<EventDetailsViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();



            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<EventsPage>();
            builder.Services.AddTransient<ManageEventPage>();
            builder.Services.AddTransient<EventDetailsPage>();
            builder.Services.AddTransient<ProfilePage>();



            builder.Services.AddSingleton<AppShell>();

            return builder.Build();
        }
    }
}
