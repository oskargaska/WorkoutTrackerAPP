
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using WorkoutTrackerAPP.DatabaseEntities;
using WorkoutTrackerAPP.Interfaces;


namespace WorkoutTrackerAPP.Services
{
    internal class DatabaseConnectionService : IDatabaseConnection
    {
        private SQLiteAsyncConnection? _connection;

        public async Task InitializeAsync()
        {
            Debug.WriteLine("InitializedAsync started");

            try
            {
                if (_connection != null) return;

                
                var dataDir = FileSystem.AppDataDirectory;
                var databasePath = Path.Combine(dataDir, "WorkoutTracker.db");


                // Checks if database is populated. Delets if is empty
                if (File.Exists(databasePath))
                {
                    // Check if database has exercises before deleting
                    var tempConnection = new SQLiteAsyncConnection(databasePath);
                    var count = await tempConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Exercises");

                    Debug.WriteLine($"Database has {count} exercises");

                    if (count == 0)
                    {
                        Debug.WriteLine("Database is empty, deleting...");
                        File.Delete(databasePath);
                    }
                    else
                    {
                        Debug.WriteLine("Database has data, keeping it");
                    }
                }
                
                // If database doesn't exist create a new one and copy the data to it from RAW file.
                if (!File.Exists(databasePath))
                {
                    Debug.WriteLine("Database didn't exist, copied from RAW");
                    using var stream = await FileSystem.OpenAppPackageFileAsync("WorkoutTracker.db");
                    using var fileStream = File.Create(databasePath);
                    await stream.CopyToAsync(fileStream);
                }
                
                
                var options = new SQLiteConnectionString(databasePath, true);
                _connection = new SQLiteAsyncConnection(options);
                
                
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new InvalidOperationException("Cannot initialize the database");
            }
            
        }

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            // Dangerous because of infinite loops. Will stay for now. Not necessarily needed for local database. It never looses conneciton.
            /*
            while(_connection == null)
            {
                await InitializeAsync();
                
            }
            */

            if (_connection != null)
            {
                return _connection;
            }
            else
            {
                await InitializeAsync();
                
            }
            if (_connection == null)
            {
                throw new InvalidOperationException("Failed to initialize database connection");
            }
            return _connection;

        }
    }
}
