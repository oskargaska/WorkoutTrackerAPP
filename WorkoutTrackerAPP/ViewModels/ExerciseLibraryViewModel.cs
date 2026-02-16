using AndroidX.Lifecycle;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Models;

namespace WorkoutTrackerAPP.ViewModels
{
    public partial class ExerciseLibraryViewModel : ObservableObject
    {
        private readonly IExercises _exercises;

        private const int PageSize = 20;
        private int _currentPage = 0;

        public ObservableCollection<ExerciseDTO> FilteredExercises { get; } = new();

        private bool _isLoadingMore;
        public ExerciseLibraryViewModel(IExercises exercises)
        {
            _exercises = exercises;
            _ = LoadMoreItems();

            
        }

        [ObservableProperty]
        private bool isFilterPanelVisible = false;


        [RelayCommand]
        async Task LoadMoreItems()
        {
            if (_isLoadingMore) return;

            _isLoadingMore = true;

            var itemsToLoad = await Task.Run(() => _exercises.Exercises
                            .Skip(_currentPage * PageSize)
                            .Take(PageSize)
                            .ToList()
                    );

            foreach (var item in itemsToLoad)
            {
                FilteredExercises.Add(item);
                //Debug.WriteLine($"{item.Name}");
            }

            _currentPage++;
            _isLoadingMore = false;
        }


        [RelayCommand]
        void ToggleFilters()
        {
            IsFilterPanelVisible = !IsFilterPanelVisible;
        }

    }


}
