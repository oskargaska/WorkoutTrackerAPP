using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Models;
using WorkoutTrackerAPP.Messages;
using System.Diagnostics;


namespace WorkoutTrackerAPP.ViewModels
{
    public partial class ExercisePickerViewModel : ObservableObject
    {
        private readonly IExercises _exercises;

        [ObservableProperty]
        private string searchText = "";

        public ObservableCollection<ExerciseDTO> FilteredExercises { get; } = new();

        public ExercisePickerViewModel(IExercises exercises)
        {
            _exercises = exercises;
            ApplyFilter();
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = _exercises.Exercises.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(e =>
                    e.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            FilteredExercises.Clear();
            foreach (var exercise in filtered)
                FilteredExercises.Add(exercise);
        }

        [RelayCommand]
        async Task SelectExercise(ExerciseDTO exercise)
        {
            // Send selected exercise back
            WeakReferenceMessenger.Default.Send(new MExerciseSelectedMessage { Exercise = exercise });
            await Shell.Current.Navigation.PopAsync();  
            //Debug.WriteLine("Should go back");

        }
    }
}
