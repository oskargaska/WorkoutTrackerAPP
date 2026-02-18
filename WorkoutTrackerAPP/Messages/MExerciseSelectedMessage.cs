using System;
using System.Collections.Generic;
using System.Text;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.Messages
{
    public class ExerciseSelectedMessage
    {
        public ExerciseDTO Exercise { get; set; }
    }
}
