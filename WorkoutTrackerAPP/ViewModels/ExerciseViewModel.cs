using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.ViewModels
{
    public partial class ExerciseViewModel : ObservableObject
    {
        private IExercises _exercises;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NumberedInstructions))]
        public ExerciseDTO exercise = new();
        public ExerciseViewModel(IExercises exercises)
        {
            _exercises = exercises;

        }

        public List<string> NumberedInstructions =>
                             Exercise?.Instructions?
                                .Select((instruction, index) => $"{index + 1}. {instruction}")
                                .ToList() ?? new List<string>();


        public async Task LoadExerciseAsync(string exerciseId)
        {
            if (string.IsNullOrWhiteSpace(exerciseId))
                return;


            var exerciseCheck = _exercises.Exercises.FirstOrDefault(e => e.Id == exerciseId);
            if (exerciseCheck == null) return;
            Exercise = exerciseCheck;
            
        }
       

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.Navigation.PopAsync();
        }
    }
}
