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
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    public partial class ActiveWorkoutViewModel : ObservableObject
    {
        private readonly IExercises _exercises;
        private readonly IWorkouts _workouts;
        private WorkoutGroupDTO _currentGroup;


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsViewMode))]
        private bool isEditMode = false;

        public bool IsViewMode => !isEditMode;

        [ObservableProperty]
        private string workoutName = "";

        [ObservableProperty]
        public string pageTitle = "Active Workout";

        public ObservableCollection<WorkoutGroupDTO> Groups { get; } = new();


        public ActiveWorkoutViewModel(IExercises exercises, IWorkouts workouts)
        {
            _exercises = exercises;
            _workouts = workouts;

        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("//workouts");

        }

        [RelayCommand]
        void ToggleEditMode()
        {
            IsEditMode = true;
        }

        [RelayCommand]
        void SaveChanges()
        {
            // Save workout changes
            IsEditMode = false;
        }

        [RelayCommand]
        void CancelEdit()
        {
            // Discard changes, reload original
            IsEditMode = false;
        }

        [RelayCommand]
        void StartWorkout()
        {
            
        }

        


        public int WorkoutId
        {
            set
            {
                var workout = _workouts.Workouts.FirstOrDefault(w => w.Id == value);
                WorkoutName = $"{workout.Name}";
                LoadWorkout(workout);
            }
        }

        public void LoadWorkout(WorkoutDTO workout)
        {
            if (workout == null) return;

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
        }
    }
}
