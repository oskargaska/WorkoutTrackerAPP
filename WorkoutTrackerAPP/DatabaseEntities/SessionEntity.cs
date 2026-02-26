using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTrackerAPP.DatabaseEntities
{
    [Table("Sessions")]
    public class SessionEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public long Date { get; set; }        // stored as ticks
        public long StartTime { get; set; }   // stored as ticks
        public long EndTime { get; set; }     // stored as ticks
        public string Json { get; set; }
    }
}
