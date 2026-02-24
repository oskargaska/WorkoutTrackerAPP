using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WorkoutTrackerAPP.Models
{
    public partial class WorkoutExerciseDTO : ObservableObject
    {
        public string ExerciseId { get; set; }    // links to DB exercise
        public string Name { get; set; }          // display name
        public EWorkoutItemType Type { get; set; } // Repetition or Timer (break)

        // Only one will be set depending on Type
        [ObservableProperty]
        public int? reps;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RemainingTimeDisplay))]
        public TimeSpan? duration;   // if timer based or a break

        [JsonIgnore]
        public bool IsRepsVisible => Reps.HasValue;

        [JsonIgnore]
        public bool IsDurationVisible => Duration.HasValue;

        [JsonIgnore]
        public WorkoutGroupDTO ParentGroup { get; set; }

        [ObservableProperty]
        [JsonIgnore]  
        private bool isActive = false;

        [JsonIgnore]
        public string RemainingTimeDisplay => Duration?.ToString(@"mm\:ss") ?? "00:00";

    }
}
