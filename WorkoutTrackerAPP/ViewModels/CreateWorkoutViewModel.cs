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
    public partial class CreateWorkoutViewModel : ObservableObject
    {

        private readonly IExercises _exercises;
        private readonly IWorkouts _workouts;
        private WorkoutGroupDTO _currentGroup;

        [ObservableProperty]
        private string workoutName = "";

        public ObservableCollection<WorkoutGroupDTO> Groups { get; } = new();

        public CreateWorkoutViewModel(IExercises exercises, IWorkouts workouts)
        {

            _exercises = exercises;
            _workouts = workouts;

            WeakReferenceMessenger.Default.Register<ExerciseSelectedMessage>(this, (recipient, message) =>
            {

                OnExerciseSelected(message.Exercise);
                Debug.WriteLine($"{message.Exercise.Name}");
            });
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("//workouts");

        }
        [RelayCommand]
        async Task AddGroup()
        {
            var group = new WorkoutGroupDTO
            {
                Name = $"Group {Groups.Count + 1}"
            };
            Groups.Add(group);
        }

        [RelayCommand]
        async Task AddItemToGroup(WorkoutGroupDTO group)
        {
            if(group.Items.Count == 0)
            {
                await AddExerciseToGroup(group);
                return;
            }

            var choice = await App.Current.MainPage.DisplayActionSheetAsync(
                "Add to group",
                "Cancel",
                null,
                "Exercise",
                "Break Timer");

            if (choice == "Exercise")
                await AddExerciseToGroup(group);
            else if (choice == "Break Timer")
                AddBreakToGroup(group);
        }

        private async Task AddExerciseToGroup(WorkoutGroupDTO group)
        {
            _currentGroup = group;
            var picker = App.Current.Handler.MauiContext.Services.GetRequiredService<ExercisePickerView>();
            await Shell.Current.Navigation.PushAsync(picker);
        }   

        private async void OnExerciseSelected(ExerciseDTO exercise)
        {
            if (_currentGroup == null) return;

            var choice = await App.Current.Windows[0].Page.DisplayActionSheetAsync(
                $"How to track {exercise.Name}?",
                "Cancel",
                null,
                "Reps",
                "Timer");

            if (choice == "Reps")
            {
                var item = new WorkoutExerciseDTO
                {
                    ExerciseId = exercise.Id,
                    Name = exercise.Name,
                    Type = EWorkoutItemType.Exercise,
                    Reps = 0
                };
                _currentGroup.Items.Add(item);
            }
            else if (choice == "Timer")
            {
                var item = new WorkoutExerciseDTO
                {
                    ExerciseId = exercise.Id,
                    Name = exercise.Name,
                    Type = EWorkoutItemType.Exercise,
                    Duration = TimeSpan.FromSeconds(30)
                };
                _currentGroup.Items.Add(item);
            }

            _currentGroup = null;
        }

        private void AddBreakToGroup(WorkoutGroupDTO group)
        {
            var breakItem = new WorkoutExerciseDTO
            {
                ExerciseId = null,
                Name = "Rest",
                Type = EWorkoutItemType.Timer,
                Duration = TimeSpan.FromSeconds(60)
            };
            group.Items.Add(breakItem);
        }

        [RelayCommand]
        void RemoveGroup(WorkoutGroupDTO group)
        {
            Groups.Remove(group);
        }

        [RelayCommand]
        void RemoveItemFromGroup((WorkoutGroupDTO group, WorkoutExerciseDTO item) parameters)
        {
            parameters.group.Items.Remove(parameters.item);
        }

        [RelayCommand]
        async Task SaveWorkout()
        {
            if (string.IsNullOrWhiteSpace(WorkoutName))
            {
                await App.Current.Windows[0].Page.DisplayAlertAsync("Error", "Please enter a workout name", "OK");
                return;
            }

            if(Groups.Count == 0)
            {
                await App.Current.Windows[0].Page.DisplayAlertAsync("Error", "Please add an exercise", "OK");
                return;
            }

            var workout = new WorkoutDTO
            {
                Name = WorkoutName,
                Groups = Groups.ToList()
            };

            await _workouts.AddWorkoutAsync(workout);
            Debug.WriteLine($"{_workouts.Workouts.Count}");
            await Shell.Current.GoToAsync("//workouts");
        }

        [RelayCommand]
        void MoveGroupUp(WorkoutGroupDTO group)
        {
            var index = Groups.IndexOf(group); 
            if(index > 0)
            {
                Groups.Move(index, index - 1);
            }
        }

        [RelayCommand]
        void MoveGroupDown(WorkoutGroupDTO group)
        {
            var index = Groups.IndexOf(group);
            if(index < Groups.Count - 1)
            {
                Groups.Move(index, index + 1);
            }
        }

        [RelayCommand]
        void MoveItemUp((WorkoutGroupDTO group, WorkoutExerciseDTO item) parameters)
        {
            var index = parameters.group.Items.IndexOf(parameters.item);
            if(index > 0)
            {
                parameters.group.Items.Move(index, index - 1);
            }
        }

        [RelayCommand]
        void MoveItemDown((WorkoutGroupDTO group, WorkoutExerciseDTO item) parameters)
        {
            var index = parameters.group.Items.IndexOf(parameters.item);
            if (index < parameters.group.Items.Count -1)
            {
                parameters.group.Items.Move(index, index + 1);
            }
        }


    }
}
