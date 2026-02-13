using SQLite;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WorkoutTrackerAPP.Data
{
    public class Database
    {
        private SQLiteAsyncConnection _connection;

        public async Task InitializeAsync()
        {
            var dataDir = FileSystem.AppDataDirectory;
            var databasePath = Path.Combine(dataDir, "WorkoutTracker.db");

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

        
    }
}
