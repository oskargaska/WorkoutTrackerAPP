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

        






    }
}
