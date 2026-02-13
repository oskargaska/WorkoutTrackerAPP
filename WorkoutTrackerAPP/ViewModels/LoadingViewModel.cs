
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WorkoutTrackerAPP.Interfaces;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;



namespace WorkoutTrackerAPP.ViewModels
{
    public partial class LoadingViewModel


    {
        
        private readonly IExercises _exercises;

        public LoadingViewModel(IExercises exercises)
        {
            _exercises = exercises;
            AppearingCommand = new AsyncRelayCommand(InitializeAsync);

        }


        public ICommand AppearingCommand { get; }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            await _exercises.LoadFromDatabaseAsync();
        }

        
    }
}
