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
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    public partial class CreateWorkoutViewModel : ObservableObject
    {

        private readonly IExercises _exercises;
        private readonly IWorkouts _workouts;
        private WorkoutGroupDTO _currentGroup;

        [ObservableProperty]
        private string workoutName = "";

        [ObservableProperty]
        private string pageTitle = "Create Workout";

        private int? _editingWorkoutId;

        public ObservableCollection<WorkoutGroupDTO> Groups { get; } = new();

        public CreateWorkoutViewModel(IExercises exercises, IWorkouts workouts)
        {

            _exercises = exercises;
            _workouts = workouts;

            WeakReferenceMessenger.Default.Register<MExerciseSelectedMessage>(this, (recipient, message) =>
            {
                Debug.WriteLine("OnExerciseSelected");

                OnExerciseSelected(message.Exercise);
                //Debug.WriteLine($"{message.Exercise.Name}");
            });
            
        }

        public int WorkoutId
        {
            set
            {
                if (Groups.Count != 0) return;
                var workout = _workouts.Workouts.FirstOrDefault(w => w.Id == value);
                if (workout == null) return;
                LoadWorkoutForEditing(workout);
            }
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
                    Reps = 0,
                    Duration = null,
                    ParentGroup = _currentGroup
                    
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
                    Reps = null,
                    Duration = TimeSpan.FromSeconds(30),
                    ParentGroup = _currentGroup
                    
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
                Duration = TimeSpan.FromSeconds(60),
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

        
        private void LoadWorkoutForEditing(WorkoutDTO workout)
        {

            _editingWorkoutId = workout.Id;
            WorkoutName = workout.Name;
            //Debug.WriteLine($"{WorkoutName}");
            Groups.Clear();
            foreach (var group in workout.Groups)
            {
                // Deep copy to avoid editing the original
                var groupCopy = new WorkoutGroupDTO
                {
                    Name = group.Name
                };

                foreach (var item in group.Items)
                {
                    var itemCopy = new WorkoutExerciseDTO
                    {
                        ExerciseId = item.ExerciseId,
                        Name = item.Name,
                        Type = item.Type,
                        Reps = item.Reps,
                        Duration = item.Duration,
                        ParentGroup = groupCopy
                    };
                    groupCopy.Items.Add(itemCopy);
                }

                Groups.Add(groupCopy);
            }
            Debug.WriteLine("LoadWorkout");

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

            var workout = new WorkoutDTO
            {
                Name = WorkoutName,
                Groups = Groups.ToList()
            };

            if (_editingWorkoutId.HasValue)
            {
                workout.Id = _editingWorkoutId.Value;
                await _workouts.UpdateWorkoutAsync(workout);
            }
            else
            {
                await _workouts.AddWorkoutAsync(workout);
            }
                

            _editingWorkoutId = null;

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

        


    }
}
