using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WorkoutTrackerAPP.Enumerators;
using WorkoutTrackerAPP.Filters;

namespace WorkoutTrackerAPP.Interfaces
{
    public interface IFilters
    {

        ObservableCollection<FilterOption> Categories { get; }
         ObservableCollection<FilterOption> Forces {  get; }
         ObservableCollection<FilterOption> Levels {  get; }
         ObservableCollection<FilterOption> Equipment {  get; }
         ObservableCollection<FilterOption> PrimaryMuscles {  get; }
         ObservableCollection<FilterOption> SecondaryMuscles { get; }    
        Task LoadFromDatabaseAsync();

        void ResetAllFilters();



    }
}
