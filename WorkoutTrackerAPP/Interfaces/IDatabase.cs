using System;
using System.Collections.Generic;
using System.Text;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Interfaces
{
    public interface IDatabase
    {
        
        Task<List<ExerciseDTO>> GetExercisesAsync();

        Task<List<WorkoutDTO>> GetWorkoutsAsync();

        // AddExercise

        // RemoveExercise

        // 
    }
}
