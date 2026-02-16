
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Models;


namespace WorkoutTrackerAPP.Services
{
    internal class ExercisesService : IExercises
    {
        private readonly IDatabase _database;

        public ExercisesService(IDatabase database)
        {
            _database = database;
            
        }

        
        public ObservableCollection<ExerciseDTO> Exercises { get; } = new();

        private bool _isLoaded = false; // Makes sure it's loaded once.
        public async Task LoadFromDatabaseAsync()
        {
            //await Application.Current.Windows[0].Page.DisplayAlertAsync("Ble", "ble", "ble");

            if (_isLoaded) return;
            _isLoaded = true;

            // Connect to database and populate the collection

            var exercises = await _database.GetExercisesAsync();

            foreach(var exercise in exercises)
            {
                Exercises.Add(exercise);
                //Debug.WriteLine($"{exercise.Name}");
            }           
        }
    }
}
