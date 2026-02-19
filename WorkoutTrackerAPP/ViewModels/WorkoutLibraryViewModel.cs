using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Messages;
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

        [RelayCommand]
        async Task EditWorkout(WorkoutDTO workout)
        {
            //Debug.WriteLine("Called EditWorkout");
            //Debug.WriteLine($"{workout.Id}");

            await Shell.Current.GoToAsync(
            "///createWorkout",
            new Dictionary<string, object>
            {
                ["WorkoutId"] = workout.Id
            });

        }


        [RelayCommand]
        async Task DeleteWorkout(WorkoutDTO workout)
        {
            var choice = await App.Current.Windows[0].Page.DisplayActionSheetAsync(
                "Do you really want to remove that workout?",
                "Cancel",
                null,
                "Yes",
                "No");

            if(choice == "Yes")
            {
                await _workouts.DeleteWorkoutAsync(workout);
                
                return;
            }
            return;
        }
    }
}
