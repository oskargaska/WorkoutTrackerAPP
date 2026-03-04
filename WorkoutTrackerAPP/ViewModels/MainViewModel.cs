using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using WorkoutTrackerAPP.Interfaces;

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
            await Shell.Current.GoToAsync("//exercises");
        }

        [RelayCommand]
        async Task NavigateToWorkouts()
        {
            await Shell.Current.GoToAsync("//workouts");
        }

        [RelayCommand]
        async Task NavigateToHistory()
        {
            await Shell.Current.GoToAsync("//history");
        }

        [RelayCommand]
        async Task ExitApp()
        {
            bool confirm = await App.Current.Windows[0].Page.DisplayAlertAsync(
                "Exit",
                "Are you sure you want to exit?",
                "Yes",
                "No");

            if (confirm)
                Application.Current.Quit();
        }
    }
}
