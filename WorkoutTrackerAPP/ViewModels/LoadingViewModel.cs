
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WorkoutTrackerAPP.Interfaces;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WorkoutTrackerAPP.Models;
using System.Diagnostics;



namespace WorkoutTrackerAPP.ViewModels
{
    public partial class LoadingViewModel


    {
        
        private readonly IExercises _exercises;

        public LoadingViewModel(IExercises exercises)
        {
            
            _exercises = exercises;
            
            

        }

        public ObservableCollection<ExerciseDTO> Exercises => _exercises.Exercises;
        

        [RelayCommand]
        public async Task InitializeAsync()
        {
            await _exercises.LoadFromDatabaseAsync();
        }

        
    }
}
