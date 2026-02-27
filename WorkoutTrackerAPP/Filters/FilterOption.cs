using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using WorkoutTrackerAPP.Enumerators;

namespace WorkoutTrackerAPP.Filters
{
    public partial class FilterOption : ObservableObject
    {
        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private EFilterCategory category;
    }
}
