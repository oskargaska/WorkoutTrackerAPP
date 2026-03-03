using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Interfaces
{
    public interface ISessions
    {
        ObservableCollection<SessionDTO> Sessions { get; }
        Task SaveWorkoutAsSession(SessionDTO session);

        Task LoadFromDatabaseAsync();
    }
}
