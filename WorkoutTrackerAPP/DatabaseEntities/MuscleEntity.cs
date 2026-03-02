using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTrackerAPP.DatabaseEntities
{
    [Table("Muscles")]
    public class MuscleEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }    
    }
}
