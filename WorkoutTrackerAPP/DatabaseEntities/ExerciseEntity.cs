using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace WorkoutTrackerAPP.DatabaseEntities
{
    [Table("Exercises")]
    public class ExerciseEntity
    {
        [PrimaryKey]
        public string Id { get; set; } 
        public string Json { get; set; }    
    }
}
