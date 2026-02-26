using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
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
        private readonly ISessions _sessions;
        private WorkoutGroupDTO _currentGroup;
        private IDispatcherTimer _timer;
        private WorkoutExerciseDTO _currentExercise;
        private int _currentGroupIndex;
        private int _currentItemIndex;
        private int _workoutId;

        private DateTime _startTime;
        private DateTime _endTime;


        [ObservableProperty]
        private bool isWorkoutActive = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsViewMode))]
        private bool isEditMode = false;
        public bool IsViewMode => !isEditMode;

        public bool IsWorkoutEdited = false;


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStop))]
        public bool isStart = false;
        public bool IsStop => !isStart;


        [ObservableProperty]
        private string workoutName = "";
        private string _workoutNameSnapshot = "";

        [ObservableProperty]
        public string pageTitle = "Active Workout";

        public ObservableCollection<WorkoutGroupDTO> Groups { get; } = new();
        private readonly List<WorkoutGroupDTO> _originalGroups = new();
        private readonly List<WorkoutGroupDTO> _snapshotGroups = new();
        private readonly WorkoutDTO _workoutSnapshot = new();


        public ActiveWorkoutViewModel(IExercises exercises, IWorkouts workouts, ISessions sessions)
        {
            _exercises = exercises;
            _workouts = workouts;
            _sessions = sessions;

            WeakReferenceMessenger.Default.Register<MExerciseSelectedMessage>(this, (recipient, message) =>
            {
                OnExerciseSelected(message.Exercise, message.IsReps);
                //Debug.WriteLine($"{message.Exercise.Name}");
            });
            _sessions = sessions;
        }

        // FUNCTIONS TO LOAD A SELECTED WORKOUT
        public int WorkoutId
        {
            set
            {
                if (_workoutId == value)
                    return;

                _workoutId = value;
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
            _originalGroups.Clear();

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
                        Duration = item.MaxDuration,
                        MaxDuration = item.MaxDuration,
                        ParentGroup = groupCopy
                    };
                    groupCopy.Items.Add(itemCopy);
                }
                _originalGroups.Add(groupCopy);
            }

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
                        Duration = item.MaxDuration,
                        MaxDuration = item.MaxDuration,
                        ParentGroup = groupCopy
                    };
                    groupCopy.Items.Add(itemCopy);
                }
                Groups.Add(groupCopy);
            }
        }

        // FUNCTIOS TO CHANGE PAGE'S FUNCTION
        [RelayCommand]
        async Task GoBack()
        {
            await DisableAllActivitiesInExercises(); 
            await AskIfSaveWorkout();
            await Shell.Current.GoToAsync("//workouts");

        }




        // FUNCTIONS TO START, STOP AND NAVIGATE THROUGH ACTIVE WORKOUT

        [RelayCommand]
        async Task ToggleEditMode()
        {
            IsEditMode = true;

            await StopWorkout();
                        
            await SnapshotGroups();
            
        }

        async Task StopWorkout()
        {
            if (IsWorkoutActive == false) return;
            else
            {
                if(_currentExercise != null)
                {
                    if (_currentExercise.Duration.HasValue)
                    {
                        _timer?.Stop();
                    }
                }
            }
        }

        async Task RestartWorkout()
        {
            if (IsWorkoutActive == false) return;
            else
            {
                if (_currentExercise != null)
                {
                    await ResetIndexesOnEdit();

                    if (_currentExercise.Duration.HasValue)
                    {
                        _timer?.Start();
                    }
                }
            }
        }

        [RelayCommand]
        async Task CancelEdit()
        {
            // Discard changes, reload original
            IsEditMode = false;

            await ResetGroups();
            await RestartWorkout();

            
        }

        async Task ResetGroups()
        {
            Groups.Clear();
            foreach (var group in _snapshotGroups)
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
                        ParentGroup = groupCopy,
                        IsActive = item.IsActive
                    };
                    if (itemCopy.IsActive)
                    {
                        _currentExercise = itemCopy;
                    }
                    
                    groupCopy.Items.Add(itemCopy);
                }
                Groups.Add(groupCopy);
            }
            WorkoutName = _workoutNameSnapshot;
        }

        async Task SnapshotGroups()
        {
            _snapshotGroups.Clear();

            _workoutNameSnapshot = WorkoutName;

            foreach (var group in Groups)
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
                        MaxDuration = item.MaxDuration,
                        ParentGroup = groupCopy,
                        IsActive = item.IsActive
                    };
                    
                    groupCopy.Items.Add(itemCopy);
                }
                _snapshotGroups.Add(groupCopy);
            }
        }

        [RelayCommand]
        void StartWorkout()
        {
            IsStart = true;

            if (Groups.Count == 0)
                return;

            IsWorkoutActive = true;
            _currentGroupIndex = 0;
            _currentItemIndex = 0;
            _startTime = DateTime.Now;
            StartCurrentExercise();
        }

        [RelayCommand]
        async Task SaveEditChanges()
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
            // Save workout changes
            IsEditMode = false;
            IsWorkoutEdited = true;
            await RestartWorkout();
        }

        async Task ResetIndexesOnEdit()
        {
            for (int g = 0; g < Groups.Count; g++)
            {
                var index = Groups[g].Items.IndexOf(Groups[g].Items.FirstOrDefault(x => x.IsActive));
                if (index >= 0)
                {
                    _currentGroupIndex = g;
                    _currentItemIndex = index;
                    _currentExercise = Groups[g].Items[index];
                    return;
                }
            }
        }




        // WORKOUT ITERATION LOGIC

        [RelayCommand]
        async Task NextExercise()
        {
            // Stop current timer
            _timer?.Stop();

            // Deactivate current
            if (_currentExercise != null)
            {
                _currentExercise.IsActive = false;
            }
                

            // Move to next
            _currentItemIndex++;

            if (_currentItemIndex >= Groups[_currentGroupIndex].Items.Count)
            {
                _currentItemIndex = 0;
                _currentGroupIndex++;

                // Check if workout is complete
                if (_currentGroupIndex >= Groups.Count || Groups[_currentGroupIndex].Items.Count == 0)
                {
                    await CompleteWorkout();
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
            if (!exercise.Duration.HasValue) return;

            var delta = TimeSpan.FromSeconds(10);

            exercise.Duration += delta;        
            exercise.MaxDuration += delta;     
        }

        [RelayCommand]
        void RemoveSeconds(WorkoutExerciseDTO exercise)
        {
            if (!exercise.Duration.HasValue) return;

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
        async Task AddExerciseToGroup(WorkoutGroupDTO group)
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
                    Duration = null,
                    MaxDuration = null,
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
                    Duration = TimeSpan.FromSeconds(30),
                    MaxDuration = TimeSpan.FromSeconds(30),
                    ParentGroup = _currentGroup
                };
                _currentGroup.Items.Add(item);
            }
            _currentGroup = null;
        }

        [RelayCommand]
        async Task AddBreakToGroup(WorkoutGroupDTO group)
        {
            _currentGroup = group;
            var breakItem = new WorkoutExerciseDTO
            {
                ExerciseId = null,
                Name = "Rest",
                Type = EWorkoutItemType.Timer,
                Duration = TimeSpan.FromSeconds(60),
                MaxDuration = TimeSpan.FromSeconds(60),
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





        // WORKOUT ENDED OR STOPPED
        [RelayCommand]
        async Task CompleteWorkout()
        {
            IsStart = false;
            IsWorkoutActive = false;
            _timer?.Stop();

            if (_currentExercise != null)
                _currentExercise.IsActive = false;

            _currentGroupIndex = 0;
            _currentItemIndex = 0;
            _endTime = DateTime.Now;

            await SnapshotWorkoutOnComplete();
            await SaveWorkoutAsSession();



            await GoBack();

            await App.Current.Windows[0].Page.DisplayAlertAsync("Complete", "Workout finished!", "OK");
        }

        async Task SaveWorkoutAsSession()
        {
            var session = new SessionDTO
            {
                StartTime = _startTime,
                EndTime = _endTime,
                Date = DateTime.Now,
                WorkoutSnapshot = _workoutSnapshot

            };

            await _sessions.SaveWorkoutAsSession(session);

        }
        async Task SnapshotWorkoutOnComplete()
        {
            _snapshotGroups.Clear();
            _workoutSnapshot.Name = WorkoutName;

            foreach (var group in Groups)
            {
                // Deep copy to avoid editing the original
                
                var groupCopy = new WorkoutGroupDTO
                {
                    Name = group.Name
                };

                foreach (var item in group.Items)
                {
                    TimeSpan? workedDuration = null;

                    if(item.Duration != null)
                    {
                        workedDuration = item.MaxDuration - item.Duration;
                        if(workedDuration < TimeSpan.Zero)
                        {
                            workedDuration = TimeSpan.Zero;
                        }
                    }
                    var itemCopy = new WorkoutExerciseDTO
                    {
                        ExerciseId = item.ExerciseId,
                        Name = item.Name,
                        Type = item.Type,
                        Reps = item.Reps,
                        MaxDuration = workedDuration,
                        ParentGroup = groupCopy
                    };

                    groupCopy.Items.Add(itemCopy);
                }
                _snapshotGroups.Add(groupCopy);
            }
            _workoutSnapshot.Groups = _snapshotGroups;

        }

        async Task AskIfSaveWorkout()
        {
            if (!IsWorkoutEdited) return;

            var choice = await App.Current.MainPage.DisplayActionSheetAsync(
                "You edited the workout, do you want to save it or not?",
                "Cancel",
                null,
                "Yes",
                "No");

            if (choice == "Yes")
                await AskIfSaveWorkoutAsNewOrOld();
            else if (choice == "No")
                return;
        }

        async Task AskIfSaveWorkoutAsNewOrOld()
        {
            var choice = await App.Current.MainPage.DisplayActionSheetAsync(
                "Save it as New Workout or overwrite current Workout?",
                "Cancel",
                null,
                "New",
                "Overwrite");

            if (choice == "New")
                await SaveWorkoutAsNew();
            else if (choice == "Overwrite")
                await SaveWorkoutOverwrite();
        }

        async Task SaveWorkoutAsNew()
        {
            if (Groups.Count == 0) return;
            var workout = new WorkoutDTO
            {
                Name = WorkoutName,
                Groups = Groups.ToList(),
            };
            await _workouts.AddWorkoutAsync(workout);

        }

        async Task SaveWorkoutOverwrite()
        {
            if (Groups.Count == 0) return;

            var workout = new WorkoutDTO
            {
                Id = _workoutId,
                Name = WorkoutName,
                Groups = Groups.ToList(),

            };
            await _workouts.UpdateWorkoutAsync(workout);
        }

        async Task DisableAllActivitiesInExercises()
        {
            foreach (var group in Groups)
            {
                if (group == null) continue;
                foreach (var exercise in group.Items)
                {
                    exercise.IsActive = false;
                }
            }
        }
    }
}
