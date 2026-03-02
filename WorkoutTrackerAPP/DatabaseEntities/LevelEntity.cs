using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTrackerAPP.DatabaseEntities
{
    [Table("Levels")]
    public class LevelEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
