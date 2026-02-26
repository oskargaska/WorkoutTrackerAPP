using System;
using System.Collections.Generic;
using System.Text;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Interfaces
{
    public interface ISessions
    {

        Task SaveWorkoutAsSession(SessionDTO session);

        Task LoadFromDatabaseAsync();
    }
}
