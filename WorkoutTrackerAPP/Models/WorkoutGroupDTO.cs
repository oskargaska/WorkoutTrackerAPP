using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WorkoutTrackerAPP.Models
{
    public class WorkoutGroupDTO
    {
        public string Name { get; set; }                          // "Chest", "Stretching" etc
        public ObservableCollection<WorkoutExerciseDTO> Items { get; set; } = new();
    }
}
