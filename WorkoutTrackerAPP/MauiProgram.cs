using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Services;
using WorkoutTrackerAPP.ViewModels;


namespace WorkoutTrackerAPP
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
            builder.Services.AddSingleton<IExercises, ExercisesService>();
            builder.Services.AddSingleton<IDatabase, DatabaseService>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<LoadingViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
