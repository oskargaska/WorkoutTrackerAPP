using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.ViewModels
{
    public partial class WorkoutLibraryViewModel : ObservableObject
    {
        private readonly IWorkouts _workouts;

        public ObservableCollection<WorkoutDTO> Workouts => _workouts.Workouts;

        public WorkoutLibraryViewModel(IWorkouts workouts)
        {
            _workouts = workouts;


        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("//main");

        }

        [RelayCommand]
        async Task AddNewWorkout()
        {

            //App.Current.Windows[0].Page.DisplayAlertAsync("Add New Workout", "Button has been pressed", "Close");

            await Shell.Current.GoToAsync("///createWorkout");

        }
    }
}
