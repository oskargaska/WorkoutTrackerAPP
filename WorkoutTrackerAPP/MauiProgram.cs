using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Services;
using WorkoutTrackerAPP.ViewModels;
using WorkoutTrackerAPP.Views;
using Syncfusion.Maui.Toolkit.Hosting;




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
                .ConfigureSyncfusionToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IExercises, ExercisesService>();
            builder.Services.AddSingleton<IWorkouts, WorkoutsService>();
            builder.Services.AddSingleton<IDatabaseConnection, DatabaseConnectionService>();
            builder.Services.AddSingleton<IDatabase, DatabaseService>();
            builder.Services.AddSingleton<ISessions, SessionsService>();

            builder.Services.AddSingleton<IFilters, FiltersService>(); //Could be transient, but ExercisePicker is never resetted while picking exercises, so it will use reset function anyway. No time to play around.

            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<MainView>();

            builder.Services.AddTransient<ExerciseLibraryViewModel>();
            builder.Services.AddTransient<ExerciseLibraryView>();

            builder.Services.AddTransient<WorkoutLibraryViewModel>();
            builder.Services.AddTransient<WorkoutLibraryView>();

            builder.Services.AddTransient<CreateWorkoutViewModel>();
            builder.Services.AddTransient<CreateWorkoutView>();

            builder.Services.AddTransient<ExercisePickerView>();
            builder.Services.AddTransient<ExercisePickerViewModel>();

            builder.Services.AddTransient<ActiveWorkoutView>();
            builder.Services.AddTransient<ActiveWorkoutViewModel>();

            builder.Services.AddTransient<HistoryView>();
            builder.Services.AddTransient<HistoryViewModel>();

            builder.Services.AddTransient<SessionView>();
            builder.Services.AddTransient<SessionViewModel>();

            builder.Services.AddTransient<ExerciseView>();
            builder.Services.AddTransient<ExerciseViewModel>();



#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Pre-initialize database on background thread
            _ = Task.Run(async () =>
            {
                var database = app.Services.GetRequiredService<IDatabaseConnection>();
                var exercises = app.Services.GetRequiredService<IExercises>();
                var workouts = app.Services.GetRequiredService<IWorkouts>();
                var sessions = app.Services.GetRequiredService<ISessions>();
                var filters = app.Services.GetRequiredService<IFilters>();
                await database.InitializeAsync();
                await exercises.LoadFromDatabaseAsync();
                await workouts.LoadFromDatabaseAsync();
                await sessions.LoadFromDatabaseAsync();
                await filters.LoadFromDatabaseAsync();


            });

            return app;
        }
    }
}
