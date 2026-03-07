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
using WorkoutTrackerAPP.Views;

namespace WorkoutTrackerAPP.ViewModels
{
    public partial class WorkoutLibraryViewModel : ObservableObject
    {
        private readonly IWorkouts _workouts;

        private object? _selectedWorkout;
        public ObservableCollection<WorkoutDTO> Workouts => _workouts.Workouts;

        public WorkoutLibraryViewModel(IWorkouts workouts)
        {
            _workouts = workouts;
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.Navigation.PopAsync();

        }

        [RelayCommand]
        async Task AddNewWorkout()
        {
            var page = App.Current.Handler.MauiContext.Services.GetRequiredService<CreateWorkoutView>();
            await Shell.Current.Navigation.PushAsync(page);

        }

        [RelayCommand]
        async Task DeleteWorkout(WorkoutDTO workout)
        {
            var choice = await Shell.Current.CurrentPage.DisplayActionSheetAsync(
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

        [RelayCommand]
        async Task WorkoutSelected(WorkoutDTO workout)
        {
            var page = App.Current.Handler.MauiContext.Services.GetRequiredService<ActiveWorkoutView>();
            var vm = (ActiveWorkoutViewModel)page.BindingContext;
            vm.WorkoutId = (int)workout.Id;
            await Shell.Current.Navigation.PushAsync(page);
        }

        public object? SelectedWorkout
        {
            get => _selectedWorkout;
            set
            {
                _selectedWorkout = value;
                OnPropertyChanged();
                _selectedWorkout = null;
                OnPropertyChanged();
            }
        }

    }
}
