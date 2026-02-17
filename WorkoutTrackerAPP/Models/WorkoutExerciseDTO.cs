using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTrackerAPP.Models
{
    public class WorkoutExerciseDTO
    {
        public string ExerciseId { get; set; }    // links to DB exercise
        public string Name { get; set; }          // display name
        public WorkoutItemType Type { get; set; } // Exercise or Timer (break)

        // Only one will be set depending on Type
        public int? Reps { get; set; }            // if reps based
        public TimeSpan? Duration { get; set; }   // if timer based or a break
    }
}
