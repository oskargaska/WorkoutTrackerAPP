using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WorkoutTrackerAPP.Models
{
    public class WorkoutDTO
    {
        public int? Id { get; set; }                              // DB autoincrement
        public string Name { get; set; }                         // workout name
        public List<WorkoutGroupDTO> Groups { get; set; } = new(); // custom groups

        [JsonIgnore]
        public int TotalExercises => Groups.Sum(g => g.Items.Count(i => i.Type == EWorkoutItemType.Exercise));

        [JsonIgnore]
        public string GroupNamesPreview => string.Join(" • ", Groups.Select(g => g.Name));
    }
}
