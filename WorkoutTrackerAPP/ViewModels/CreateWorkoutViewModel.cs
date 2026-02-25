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

        [ObservableProperty]
        private string pageTitle = "Create Workout";

        public ObservableCollection<WorkoutGroupDTO> Groups { get; } = new();

        public CreateWorkoutViewModel(IExercises exercises, IWorkouts workouts)
        {

            _exercises = exercises;
            _workouts = workouts;

            WeakReferenceMessenger.Default.Register<MExerciseSelectedMessage>(this, (recipient, message) =>
            {
                OnExerciseSelected(message.Exercise, message.IsReps);
                //Debug.WriteLine($"{message.Exercise.Name}");
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

        [RelayCommand]
        private async Task AddExerciseToGroup(WorkoutGroupDTO group)
        {
            _currentGroup = group;
            var picker = App.Current.Handler.MauiContext.Services.GetRequiredService<ExercisePickerView>();
            await Shell.Current.Navigation.PushAsync(picker);
        }

        private async void OnExerciseSelected(ExerciseDTO exercise, bool isReps)
        {
            if (_currentGroup == null) return;


            if (isReps == true)
            {
                var item = new WorkoutExerciseDTO
                {
                    ExerciseId = exercise.Id,
                    Name = exercise.Name,
                    Type = EWorkoutItemType.Exercise,
                    Reps = 0,
                    MaxDuration = null,
                    Duration = null,
                    ParentGroup = _currentGroup

                };
                _currentGroup.Items.Add(item);
            }
            else if (isReps == false)
            {
                var item = new WorkoutExerciseDTO
                {
                    ExerciseId = exercise.Id,
                    Name = exercise.Name,
                    Type = EWorkoutItemType.Exercise,
                    Reps = null,
                    MaxDuration = TimeSpan.FromSeconds(30),
                    Duration = TimeSpan.FromSeconds(30),
                    ParentGroup = _currentGroup

                };
                _currentGroup.Items.Add(item);
            }

            _currentGroup = null;
        }

        [RelayCommand]
        private void AddBreakToGroup(WorkoutGroupDTO group)
        {
            var breakItem = new WorkoutExerciseDTO
            {
                ExerciseId = null,
                Name = "Rest",
                Type = EWorkoutItemType.Timer,
                Duration = TimeSpan.FromSeconds(60),
                MaxDuration = TimeSpan.FromSeconds(60),
                ParentGroup = group
                
            };
            group.Items.Add(breakItem);
        }

        [RelayCommand]
        void RemoveGroup(WorkoutGroupDTO group)
        {
            Groups.Remove(group);
        }

        [RelayCommand]
        void RemoveItemFromGroup(WorkoutExerciseDTO item)
        {
            item.ParentGroup?.Items.Remove(item);
        }

       

        [RelayCommand]
        async Task SaveWorkout()
        {
            if (string.IsNullOrWhiteSpace(WorkoutName))
            {
                await App.Current.Windows[0].Page.DisplayAlertAsync("Error", "Please enter a workout name", "OK");
                return;
            }
            if (Groups.Count == 0)
            {
                await App.Current.Windows[0].Page.DisplayAlertAsync("Error", "Add at least one group", "OK");
                return;
            }
            for (int i = Groups.Count - 1; i >= 0; i--)
            {
                if (Groups[i].Items.Count == 0)
                    Groups.RemoveAt(i);
            }
            var workout = new WorkoutDTO
            {
                Name = WorkoutName,
                Groups = Groups.ToList()
            };
            await _workouts.AddWorkoutAsync(workout);
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
        void MoveItemUp(WorkoutExerciseDTO item)
        {
            var group = item.ParentGroup;
            if (group == null) return;

            var index = group.Items.IndexOf(item);
            if (index > 0)
            {
                group.Items.Move(index, index - 1);
            }
        }

        [RelayCommand]
        void MoveItemDown(WorkoutExerciseDTO item)
        {
            var group = item.ParentGroup;
            if (group == null) return;

            var index = group.Items.IndexOf(item);
            if (index < group.Items.Count - 1)
            {
                group.Items.Move(index, index + 1);
            }
        }

        [RelayCommand]
        void AddSeconds(WorkoutExerciseDTO exercise)
        {
            if (!exercise.Duration.HasValue)
                return;

            var delta = TimeSpan.FromSeconds(10);

            exercise.Duration += delta;        // affects current session
            exercise.MaxDuration += delta;     // affects workout definition

        }

        [RelayCommand]
        void RemoveSeconds(WorkoutExerciseDTO exercise)
        {
            if (!exercise.Duration.HasValue)
                return;

            var delta = TimeSpan.FromSeconds(10);

            var newDuration = exercise.Duration.Value - delta;
            if (newDuration < TimeSpan.Zero)
                newDuration = TimeSpan.Zero;

            var newMax = exercise.MaxDuration - delta;
            if (newMax < TimeSpan.Zero)
                newMax = TimeSpan.Zero;

            exercise.Duration = newDuration;
            exercise.MaxDuration = newMax;

        }

        [RelayCommand]
        void AddReps(WorkoutExerciseDTO exercise)
        {
            if (exercise.Reps.HasValue)
                exercise.Reps += 1;
        }

        [RelayCommand]
        void RemoveReps(WorkoutExerciseDTO exercise)
        {
            if (exercise.Reps.HasValue && exercise.Reps.Value >= 1)
                exercise.Reps -= 1;
        }




    }
}
