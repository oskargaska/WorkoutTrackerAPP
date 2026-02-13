using Java.Net;
using SQLite;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using WorkoutTrackerAPP.DatabaseEntities;
using WorkoutTrackerAPP.Interfaces;


namespace WorkoutTrackerAPP.Services
{
    public class DatabaseConnectionService : IDatabaseConnection
    {
        private SQLiteAsyncConnection _connection;
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public async Task InitializeAsync()
        {
            await _initLock.WaitAsync();
            try
            {
                if (_connection != null) return;

                var dataDir = FileSystem.AppDataDirectory;
                var databasePath = Path.Combine(dataDir, "WorkoutTracker.db");

                // Copy from assets if needed
                if (!File.Exists(databasePath))
                {
                    using var stream = await FileSystem.OpenAppPackageFileAsync("WorkoutTracker.db");
                    using var fileStream = File.Create(databasePath);
                    await stream.CopyToAsync(fileStream);
                }

                var options = new SQLiteConnectionString(databasePath, true);
                _connection = new SQLiteAsyncConnection(options);

                // Verify it works
                await _connection.CreateTableAsync<ExerciseEntity>();
                
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_connection == null)
                throw new InvalidOperationException("Database not initialized");

            // Optional: Test connection validity
            try
            {
                await _connection.ExecuteScalarAsync<int>("SELECT 1");
            }
            catch (Exception ex)
            {
                
                throw new InvalidOperationException("Database connection is broken", ex);
            }

            return _connection;
        }
    }
}
