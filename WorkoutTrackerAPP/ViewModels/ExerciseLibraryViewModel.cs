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


        [ObservableProperty]
        private string searchText = "";

        private const int PageSize = 20;
        private int _currentPage = 0;

        [ObservableProperty]
        private bool isFilterPanelVisible = false;

        public ObservableCollection<ExerciseDTO> FilteredExercises { get; } = new();

        private bool _isLoadingMore;
        public ExerciseLibraryViewModel(IExercises exercises)
        {
            _exercises = exercises;
            _ = LoadMoreItems();  // Loads first 20 items

            
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("//main");

        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = _exercises.Exercises.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(e =>
                    e.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                FilteredExercises.Clear();
                _currentPage = 0;
                _ = LoadMoreItems();
                return;
            }


            FilteredExercises.Clear();
            foreach (var exercise in filtered)
                FilteredExercises.Add(exercise);
        }


        [RelayCommand]
        async Task AddNewExercise()
        {

            await App.Current.Windows[0].Page.DisplayAlertAsync("Add New Exercise", "Button has been pressed", "Close");
            
            //await Shell.Current.GoToAsync("..");

        }


        


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
