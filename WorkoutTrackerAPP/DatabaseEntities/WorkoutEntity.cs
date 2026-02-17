using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTrackerAPP.DatabaseEntities
{
    [Table("Workouts")]
    public class WorkoutEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Json { get; set; }
    }
}
