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
    public partial class ActiveWorkoutViewModel : ObservableObject
    {
        private readonly IExercises _exercises;
        private readonly IWorkouts _workouts;
        private WorkoutGroupDTO _currentGroup;
        private IDispatcherTimer _timer;
        private WorkoutExerciseDTO _currentExercise;
        private int _currentGroupIndex;
        private int _currentItemIndex;
        private List<WorkoutGroupDTO> _originalWorkoutGroups = new();

        [ObservableProperty]
        private bool isWorkoutActive = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsViewMode))]
        private bool isEditMode = false;
        public bool IsViewMode => !isEditMode;



        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStop))]
        public bool isStart = false;
        public bool IsStop => !isStart;


        [ObservableProperty]
        private string workoutName = "";

        [ObservableProperty]
        public string pageTitle = "Active Workout";

        public ObservableCollection<WorkoutGroupDTO> Groups { get; } = new();


        public ActiveWorkoutViewModel(IExercises exercises, IWorkouts workouts)
        {
            _exercises = exercises;
            _workouts = workouts;

            WeakReferenceMessenger.Default.Register<MExerciseSelectedMessage>(this, (recipient, message) =>
            {
                OnExerciseSelected(message.Exercise);
                //Debug.WriteLine($"{message.Exercise.Name}");
            });

        }


        // FUNCTIONS TO LOAD A SELECTED WORKOUT
        public int WorkoutId
        {
            set
            {
                if (Groups.Count != 0) return;
                var workout = _workouts.Workouts.FirstOrDefault(w => w.Id == value);
                if (workout == null) return;
                LoadWorkout(workout);
            }
        }

        public void LoadWorkout(WorkoutDTO workout)
        {
            WorkoutName = workout.Name;
            //Debug.WriteLine($"{WorkoutName}");
            Groups.Clear();
            _originalWorkoutGroups.Clear();
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
                _originalWorkoutGroups.Add(groupCopy);
                Groups.Add(groupCopy);
            }
            

        }



        // FUNCTIOS TO CHANGE PAGE'S FUNCTION

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("//workouts");

        }

        [RelayCommand]
        void ToggleEditMode()
        {
            IsEditMode = true;
            if(_timer != null)
            {
                _timer.Stop();

            }
            
        }


        [RelayCommand]
        async Task CancelEdit()
        {
            // Discard changes, reload original
            IsEditMode = false;
            if (_timer != null)
            {
                _timer.Start();

            }
            Groups.Clear();
            foreach(var group in _originalWorkoutGroups)
            {
                Groups.Add(group);
            }
            await Task.CompletedTask;
        }


        // FUNCTIONS TO START, STOP AND NAVIGATE THROUGH ACTIVE WORKOUT

        [RelayCommand]
        void StartWorkout()
        {
            IsStart = true;

            if (Groups.Count == 0)
                return;

            IsWorkoutActive = true;
            _currentGroupIndex = 0;
            _currentItemIndex = 0;
            StartCurrentExercise();
        }

        [RelayCommand]
        void CompleteWorkout()
        {
            IsStart = false;
            IsWorkoutActive = false;
            _timer?.Stop();

            if (_currentExercise != null)
                _currentExercise.IsActive = false;

            _currentGroupIndex = 0;
            _currentItemIndex = 0;

            // Save session, show completion message, etc.
            App.Current.MainPage.DisplayAlertAsync("Complete", "Workout finished!", "OK");
        }

        [RelayCommand]
        void NextExercise()
        {
            // Stop current timer
            _timer?.Stop();

            // Deactivate current
            if (_currentExercise != null)
                _currentExercise.IsActive = false;

            // Move to next
            _currentItemIndex++;

            if (_currentItemIndex >= Groups[_currentGroupIndex].Items.Count)
            {
                _currentItemIndex = 0;
                _currentGroupIndex++;

                // Check if workout is complete
                if (_currentGroupIndex >= Groups.Count || Groups[_currentGroupIndex].Items.Count == 0)
                {
                    CompleteWorkout();
                    return;
                }
            }

            StartCurrentExercise();
        }

        [RelayCommand]
        void PreviousExercise()
        {
            // Stop current timer
            _timer?.Stop();

            // Deactivate current
            if (_currentExercise != null)
                _currentExercise.IsActive = false;

            // Move to previous
            _currentItemIndex--;

            // Check if we need to move to previous group
            if (_currentItemIndex < 0)
            {
                _currentGroupIndex--;

                if (_currentGroupIndex < 0)
                {
                    _currentGroupIndex = 0;
                    _currentItemIndex = 0;
                }
                else
                {
                    _currentItemIndex = Groups[_currentGroupIndex].Items.Count - 1;
                }
            }

            StartCurrentExercise();
        }

        private void StartCurrentExercise()
        {
            if (_currentGroupIndex < 0 || _currentGroupIndex >= Groups.Count)
                return;

            var currentGroup = Groups[_currentGroupIndex];
            if (_currentItemIndex < 0 || _currentItemIndex >= currentGroup.Items.Count)
                return;

            // Deactivate previous
            if (_currentExercise != null)
                _currentExercise.IsActive = false;

            // Activate current from the actual Groups collection
            _currentExercise = currentGroup.Items[_currentItemIndex];
            _currentExercise.IsActive = true;

            // Only start timer if duration-based
            if (!_currentExercise.Duration.HasValue)
                return;

            _timer?.Stop();
            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_currentExercise?.Duration > TimeSpan.Zero)
            {
                _currentExercise.Duration = _currentExercise.Duration.Value.Subtract(TimeSpan.FromSeconds(1));
            }
            else
            {
                _timer?.Stop();
                // Auto-advance to next exercise
                NextExercise();
            }
        }



        // FUNCTIONS TO EDIT EXERCISE'S REPS OR DURATION

        [RelayCommand]
        void AddSeconds(WorkoutExerciseDTO exercise)
        {
            if (exercise.Duration.HasValue)
                exercise.Duration = exercise.Duration.Value.Add(TimeSpan.FromSeconds(10));
        }

        [RelayCommand]
        void RemoveSeconds(WorkoutExerciseDTO exercise)
        {
            if (exercise.Duration.HasValue && exercise.Duration.Value.TotalSeconds >= 10)
                exercise.Duration = exercise.Duration.Value.Subtract(TimeSpan.FromSeconds(10));
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


        // FUNCTIONS TO EDIT THE WORKOUT

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
            if (group.Items.Count == 0)
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
            _currentGroup = group;
            var breakItem = new WorkoutExerciseDTO
            {
                ExerciseId = null,
                Name = "Rest",
                Type = EWorkoutItemType.Timer,
                Duration = TimeSpan.FromSeconds(60),
                ParentGroup = group

            };
            _currentGroup.Items.Add(breakItem);
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
        void MoveGroupUp(WorkoutGroupDTO group)
        {
            var index = Groups.IndexOf(group);
            if (index > 0)
            {
                Groups.Move(index, index - 1);
            }
        }

        [RelayCommand]
        void MoveGroupDown(WorkoutGroupDTO group)
        {
            var index = Groups.IndexOf(group);
            if (index < Groups.Count - 1)
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
        async Task SaveChanges()
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
            // Save workout changes
            IsEditMode = false;

        }


    }
}
