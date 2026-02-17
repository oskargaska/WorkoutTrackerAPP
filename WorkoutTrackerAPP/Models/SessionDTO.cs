using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTrackerAPP.Models
{
    public class SessionDTO
    {
        public int Id { get; set; }          // DB autoincrement   // links to saved workout
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;

        public WorkoutDTO WorkoutSnapshot { get; set; }
    }
}
