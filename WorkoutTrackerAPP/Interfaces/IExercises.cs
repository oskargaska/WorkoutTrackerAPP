using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Interfaces
{
    public interface IExercises
    {
        ObservableCollection<ExerciseDTO> Exercises { get; }
        Task LoadFromDatabaseAsync();
    }
}
