using System;
using System.Collections.Generic;
using System.Text;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Messages
{
    public class MExerciseSelectedMessage
    {
        public ExerciseDTO Exercise { get; set; }
        public bool IsReps { get; set; }
    }
}
