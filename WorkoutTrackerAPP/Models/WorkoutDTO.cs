using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTrackerAPP.Models
{
    public class WorkoutDTO
    {
        public int Id { get; set; }                              // DB autoincrement
        public string Name { get; set; }                         // workout name
        public List<WorkoutGroupDTO> Groups { get; set; } = new(); // custom groups
    }
}
