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

            await Task.CompletedTask;
        }

        public async Task AddWorkoutAsync(WorkoutDTO workout)
        {
            if (workout == null) return;

            var id = await _database.AddWorkoutAsync(workout);
            
            workout.Id = id;
            Workouts.Add(workout);

            await Task.CompletedTask;
        }

        public async Task UpdateWorkoutAsync(WorkoutDTO workout)
        {
            if (workout == null) return;
            await _database.UpdateWorkoutAsync(workout);

            var existing = Workouts.FirstOrDefault(w => w.Id == workout.Id);
            if (existing != null)
            {
                var index = Workouts.IndexOf(existing);
                Workouts[index] = workout;
            }

            

            await Task.CompletedTask;

        }


        public async Task DeleteWorkoutAsync(WorkoutDTO workout)
        {
            await _database.DeleteWorkoutAsync((int)workout.Id);
            var existing = Workouts.FirstOrDefault(w => w.Id == workout.Id);
            if(existing != null)
            {
                Workouts.Remove(existing);
            }
            await Task.CompletedTask;

        }
    }
}
