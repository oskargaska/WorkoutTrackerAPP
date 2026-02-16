using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private IDatabaseConnection _databaseConnection;


        public DatabaseService(IDatabaseConnection databaseConnection) 
        {
            _databaseConnection = databaseConnection;



        }

        

        public async Task<List<ExerciseDTO>> GetExercisesAsync()
        {
            var connection = await _databaseConnection.GetConnectionAsync();
            var entities = await connection.Table<ExerciseEntity>().ToListAsync();
            var exercise = new List<ExerciseDTO>(); 

            if(entities == null)
            {
                Debug.WriteLine("Database querry returned empty list of entities");
            }
            foreach (var entity in entities)
            {
                

                if (entity.Json == null)
                {
                    continue;
                }
                else
                {
                    exercise.Add(JsonSerializer.Deserialize<ExerciseDTO>(entity.Json));
                    //Debug.WriteLine($"{exercise.Last().Name}");
                    //Debug.WriteLine($"{entity.Json}");
                    
                }
            }
            return exercise;


        }
    }
}
