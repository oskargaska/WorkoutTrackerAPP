using System;
using System.Collections.Generic;
using System.Text;
using WorkoutTrackerAPP.Interfaces;

namespace WorkoutTrackerAPP.ViewModels
{
    public class MainViewModel
    {

        private readonly IExercises _exercises;
        public MainViewModel(IExercises exercises)
        {
            _exercises = exercises;

            


        }









    }
}
