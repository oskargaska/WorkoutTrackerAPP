using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTrackerAPP.Models
{
    public partial class FilterOption : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private string category; // "Equipment", "Level", "Muscle"
    }
}
