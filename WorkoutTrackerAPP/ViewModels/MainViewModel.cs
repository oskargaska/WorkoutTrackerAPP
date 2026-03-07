using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Views;

namespace WorkoutTrackerAPP.ViewModels
{
    public partial class MainViewModel
    {

        private readonly IExercises _exercises;

        public MainViewModel(IExercises exercises)
        {
            _exercises = exercises;

        }


        [RelayCommand]
        async Task NavigateToExercises()
        {
            var page = App.Current.Handler.MauiContext.Services.GetRequiredService<ExerciseLibraryView>();
            await Shell.Current.Navigation.PushAsync(page);
        }

        [RelayCommand]
        async Task NavigateToWorkouts()
        {
            var page = App.Current.Handler.MauiContext.Services.GetRequiredService<WorkoutLibraryView>();
            await Shell.Current.Navigation.PushAsync(page);
        }

        [RelayCommand]
        async Task NavigateToHistory()
        {
            var page = App.Current.Handler.MauiContext.Services.GetRequiredService<HistoryView>();
            await Shell.Current.Navigation.PushAsync(page);
        }

        [RelayCommand]
        async Task ExitApp()
        {
            bool confirm = await Shell.Current.CurrentPage.DisplayAlertAsync(
                "Exit",
                "Are you sure you want to exit?",
                "Yes",
                "No");

            if (confirm)
                Application.Current.Quit();
        }
    }
}
