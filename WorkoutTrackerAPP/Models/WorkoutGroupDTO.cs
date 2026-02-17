using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTrackerAPP.Models
{
    public class WorkoutGroupDTO
    {
        public string Name { get; set; }                          // "Chest", "Stretching" etc
        public List<WorkoutExerciseDTO> Items { get; set; } = new(); // exercises + breaks in order
    }
}
