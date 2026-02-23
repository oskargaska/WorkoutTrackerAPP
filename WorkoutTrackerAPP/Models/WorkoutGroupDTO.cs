using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json.Serialization;

namespace WorkoutTrackerAPP.Models
{
    public partial class WorkoutGroupDTO
    {
        public string Name { get; set; }                          
        public ObservableCollection<WorkoutExerciseDTO> Items { get; set; } = new();

    }
}
