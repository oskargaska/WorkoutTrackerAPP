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
        private SQLiteAsyncConnection _connection;

        public async Task InitializeAsync()
        {
            var dataDir = FileSystem.AppDataDirectory;
            var databasePath = Path.Combine(dataDir, "WorkoutTracker.db");


            // Copy from assets if DB doesn't exist yet
            if (!File.Exists(databasePath))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("WorkoutTracker.db");
                using var fileStream = File.Create(databasePath);
                await stream.CopyToAsync(fileStream);
            }

            var key = await SecureStorage.GetAsync("dbKey");

            if (string.IsNullOrEmpty(key))
            {
                key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)); // Creates a new random key.
                await SecureStorage.SetAsync("dbKey", key);
            }

            var options = new SQLiteConnectionString(
                databasePath, // Path to database.
                true, // Save DateTime.ticks as INTEGER -> Apparently faster than TEXT.
                key: key); // Encryption key

            _connection = new SQLiteAsyncConnection(options);

            await _connection.ExecuteAsync("PRAGMA foreign_keys = ON;");
        }

        public async Task<List<ExerciseDTO>> GetExercisesAsync()
        {
            var entities = await _connection.Table<ExerciseEntity>().ToListAsync();
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
