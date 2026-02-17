using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Interfaces
{
    public interface IWorkouts
    {
        ObservableCollection<WorkoutDTO> Workouts { get; }
        Task LoadFromDatabaseAsync();
        Task AddWorkoutAsync(WorkoutDTO workout);
        Task UpdateWorkoutAsync(WorkoutDTO workout);
        Task DeleteWorkoutAsync(int id);
    }
}
