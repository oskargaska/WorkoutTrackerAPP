using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WorkoutTrackerAPP.Models
{
    public class WorkoutExerciseDTO
    {
        public string ExerciseId { get; set; }    // links to DB exercise
        public string Name { get; set; }          // display name
        public EWorkoutItemType Type { get; set; } // Repetition or Timer (break)

        // Only one will be set depending on Type
        public int? Reps { get; set; }    

        public TimeSpan? Duration { get; set; }   // if timer based or a break

        [JsonIgnore]
        public bool IsRepsVisible => Reps.HasValue;

        [JsonIgnore]
        public bool IsDurationVisible => Duration.HasValue;

        [JsonIgnore]
        public WorkoutGroupDTO ParentGroup { get; set; }


        [JsonIgnore]
        public string DurationInSeconds
        {
            get => Duration.HasValue ? ((int)Duration.Value.TotalSeconds).ToString() : "";
            set
            {
                if (int.TryParse(value, out int seconds) && seconds > 0)
                    Duration = TimeSpan.FromSeconds(seconds);
                else
                    Duration = null;
            }
        }

    }
}
