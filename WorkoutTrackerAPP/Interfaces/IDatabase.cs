using System;
using System.Collections.Generic;
using System.Text;
using WorkoutTrackerAPP.Enumerators;
using WorkoutTrackerAPP.Filters;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Interfaces
{
    public interface IDatabase
    {
        
        Task<List<ExerciseDTO>> GetExercisesAsync();

        Task<List<WorkoutDTO>> GetWorkoutsAsync();

        // AddExercise

        // RemoveExercise

        Task<int> AddWorkoutAsync(WorkoutDTO workout);

        Task UpdateWorkoutAsync(WorkoutDTO workout);

        Task DeleteWorkoutAsync(int id);

        Task<List<SessionDTO>> GetSessionsAsync();

        Task<int> AddSessionAsync(SessionDTO session);


        Task<List<FilterOption>> GetCategoriesAsync();
        Task<List<FilterOption>> GetForcesAsync();

        Task<List<FilterOption>> GetLevelsAsync();
        Task<List<FilterOption>> GetEquipmentAsync();
        Task<List<FilterOption>> GetMusclesAsync();

    }
}
