using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Services
{
    public class WorkoutsService : IWorkouts
    {
        private readonly IDatabase _database;
        private bool _isLoaded = false;

        public ObservableCollection<WorkoutDTO> Workouts { get; } = new();

        public WorkoutsService(IDatabase database)
        {
            _database = database;
        }

        public async Task LoadFromDatabaseAsync()
        {
            if (_isLoaded) return;
            _isLoaded = true;

            var workouts = await _database.GetWorkoutsAsync();
            foreach (var workout in workouts)
                Workouts.Add(workout);
        }

        public async Task AddWorkoutAsync(WorkoutDTO workout)
        {
            if (workout == null) return;
            Workouts.Add(workout);
        }
        public async Task UpdateWorkoutAsync(WorkoutDTO workout)
        {

        }
        public async Task DeleteWorkoutAsync(int id)
        {


        }
    }
}
