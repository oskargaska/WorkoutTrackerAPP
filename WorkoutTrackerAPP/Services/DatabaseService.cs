using SQLite;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WorkoutTrackerAPP.DatabaseEntities;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Services
{
    internal class DatabaseService : IDatabase
    {
        private static DatabaseConnectionService _databaseConnectionService;


        public DatabaseService(DatabaseConnectionService databaseConnectionService) 
        {
            _databaseConnectionService = databaseConnectionService;



        }

        

        public async Task<List<ExerciseDTO>> GetExercisesAsync()
        {
            var connection = await _databaseConnectionService.GetConnectionAsync();
            var entities = await connection.Table<ExerciseEntity>().ToListAsync();
            var exercise = new List<ExerciseDTO>(); 

            foreach (var entity in entities)
            {


                if (entity.Json == null)
                {
                    continue;
                }
                else
                {
                    exercise.Add(JsonSerializer.Deserialize<ExerciseDTO>(entity.Json));
                }
            }
            return exercise;


        }
    }
}
