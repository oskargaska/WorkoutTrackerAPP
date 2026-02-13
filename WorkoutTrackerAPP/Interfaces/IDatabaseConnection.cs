using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTrackerAPP.Interfaces
{
    public interface IDatabaseConnection
    {
        Task<SQLiteAsyncConnection> GetConnectionAsync();
        Task InitializeAsync();
    }
}
