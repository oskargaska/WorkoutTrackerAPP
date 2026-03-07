using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using WorkoutTrackerAPP.Filters;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Messages;
using WorkoutTrackerAPP.Models;
using WorkoutTrackerAPP.Views;


namespace WorkoutTrackerAPP.ViewModels
{
    public partial class ExercisePickerViewModel : ObservableObject
    {
        private readonly IExercises _exercises;
        private readonly IFilters _filters;

        [ObservableProperty]
        private string searchText = "";

        private const int PageSize = 20;
        private int _currentPage = 0;

        [ObservableProperty]
        private bool isFilterPanelVisible = false;

        public ObservableCollection<ExerciseDTO> FilteredExercises { get; } = new();

        private bool _isLoadingMore;

        private List<ExerciseDTO> _filteredSource = new();



        // Filter collections
        public ObservableCollection<FilterOption> AvailableEquipment => _filters.Equipment;
        public ObservableCollection<FilterOption> AvailableLevels => _filters.Levels;
        public ObservableCollection<FilterOption> AvailableForces => _filters.Forces;
        public ObservableCollection<FilterOption> AvailableCategories => _filters.Categories;
        public ObservableCollection<FilterOption> AvailablePrimaryMuscles => _filters.PrimaryMuscles;
        public ObservableCollection<FilterOption> AvailableSecondaryMuscles => _filters.SecondaryMuscles;

        public ExercisePickerViewModel(IExercises exercises, IFilters filters)
        {
            _exercises = exercises;
            _filters = filters;

            foreach (var option in AllFilters())
                option.PropertyChanged += OnFilterOptionChanged;


            ApplyFilter();

        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        private async void ApplyFilter()
        {
            // Capture current selections
            var searchText = SearchText;
            var selectedEquipment = AvailableEquipment.Where(f => f.IsSelected).Select(f => f.Name).ToHashSet();
            var selectedLevels = AvailableLevels.Where(f => f.IsSelected).Select(f => f.Name).ToHashSet();
            var selectedForces = AvailableForces.Where(f => f.IsSelected).Select(f => f.Name).ToHashSet();
            var selectedCategories = AvailableCategories.Where(f => f.IsSelected).Select(f => f.Name).ToHashSet();
            var selectedPrimary = AvailablePrimaryMuscles.Where(f => f.IsSelected).Select(f => f.Name).ToHashSet();
            var selectedSecondary = AvailableSecondaryMuscles.Where(f => f.IsSelected).Select(f => f.Name).ToHashSet();

            var filtered = await Task.Run(() =>
            {
                var query = _exercises.Exercises.AsEnumerable();

                // Search
                if (!string.IsNullOrWhiteSpace(searchText))
                    query = query.Where(e => e.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));

                // Equipment
                if (selectedEquipment.Any())
                    query = query.Where(e => selectedEquipment.Contains(e.Equipment));

                // Level
                if (selectedLevels.Any())
                    query = query.Where(e => selectedLevels.Contains(e.Level));

                // Forces
                if (selectedForces.Any())
                    query = query.Where(e => selectedForces.Contains(e.Force));

                // Category
                if (selectedCategories.Any())
                    query = query.Where(e => selectedCategories.Contains(e.Category));

                // Primary muscles (OR)
                if (selectedPrimary.Any())
                    query = query.Where(e =>
                        (e.PrimaryMuscles ?? new List<string>()).Any(pm => selectedPrimary.Contains(pm)));

                // Secondary muscles (AND)
                if (selectedSecondary.Any())
                    query = query.Where(e =>
                    {
                        var sec = e.SecondaryMuscles ?? new List<string>();
                        var secSet = new HashSet<string>(sec);
                        return selectedSecondary.All(sm => secSet.Contains(sm));
                    });

                return query.ToList();
            });

            // Update UI
            _filteredSource = filtered;
            _currentPage = 0;
            FilteredExercises.Clear();
            await LoadMoreItems();
        }


        [RelayCommand]
        async Task AddExerciseWithReps(ExerciseDTO exercise)
        {
            WeakReferenceMessenger.Default.Send(new MExerciseSelectedMessage { Exercise = exercise, IsReps = true });
            IsFilterPanelVisible = false;
            _filters.ResetAllFilters();
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        async Task AddExerciseWithDuration(ExerciseDTO exercise)
        {
            WeakReferenceMessenger.Default.Send(new MExerciseSelectedMessage { Exercise = exercise, IsReps = false });
            IsFilterPanelVisible = false;
            _filters.ResetAllFilters();
            await Shell.Current.Navigation.PopAsync();
        }


        [RelayCommand]
        async Task LoadMoreItems()
        {
            if (_isLoadingMore) return;

            _isLoadingMore = true;

            var itemsToLoad = _filteredSource
                                .Skip(_currentPage * PageSize)
                                .Take(PageSize)
                                .ToList();

            foreach (var item in itemsToLoad)
            {
                FilteredExercises.Add(item);
            }

            _currentPage++;
            _isLoadingMore = false;
        }


        [RelayCommand]
        void ToggleFilter(FilterOption option)
        {
            option.IsSelected = !option.IsSelected;
        }

        [RelayCommand]
        void ToggleFilterPanel()
        {

            IsFilterPanelVisible = !IsFilterPanelVisible;

        }

        private void OnFilterOptionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FilterOption.IsSelected))
                ApplyFilter();
        }

        [RelayCommand]
        void ClearFilters()
        {
            foreach (var option in AllFilters())
                option.IsSelected = false;

            ApplyFilter(); // Only one call needed
        }

        private IEnumerable<FilterOption> AllFilters() =>
            AvailableEquipment
                .Concat(AvailableLevels)
                .Concat(AvailableForces)
                .Concat(AvailableCategories)
                .Concat(AvailablePrimaryMuscles)
                .Concat(AvailableSecondaryMuscles);


        [RelayCommand]
        async Task SelectExercise(ExerciseDTO exercise)
        {
            var exerciseView = App.Current.Handler.MauiContext.Services.GetRequiredService<ExerciseView>();
            exerciseView.SetExercise(exercise.Id);
            await Shell.Current.Navigation.PushAsync(exerciseView);

        }
    }
}
