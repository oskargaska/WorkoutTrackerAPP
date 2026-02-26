using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Services
{
    internal class SessionsService : ISessions
    {
        private readonly IDatabase _database;
        private bool _isLoaded = false;

        public ObservableCollection<SessionDTO> Sessions { get; } = new();

        public SessionsService(IDatabase database)
        {
            _database = database;
        }
         public async Task SaveWorkoutAsSession(SessionDTO session)
        {
            if (session == null) return;

            var id = await _database.AddSessionAsync(session);

            session.Id = id;
            Sessions.Add(session);

            await Task.CompletedTask;


        }

        public async Task LoadFromDatabaseAsync()
        {
            if (_isLoaded) return;
            _isLoaded = true;

            var sessions = await _database.GetSessionsAsync();
            foreach (var session in sessions)
                Sessions.Add(session);

            await Task.CompletedTask;
        }
    }
}
