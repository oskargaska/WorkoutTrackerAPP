using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTrackerAPP.Models
{
    public class ExerciseDTO
    {
        public string Name { get; set; }
        public string Force { get; set; }
        public string Level { get; set; }
        public string Mechanic { get; set; }
        public string Equipment { get; set; }
        public List<string> PrimaryMuscles { get; set; }
        public List<string> SecondaryMuscles { get; set; }
        public List<string> Instructions { get; set; }
        public string Category { get; set; }
        public List<string> Images { get; set; }
        public string Id { get; set; }
    }
}
