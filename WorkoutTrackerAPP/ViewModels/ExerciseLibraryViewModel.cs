using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using WorkoutTrackerAPP.Filters;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.Models;
using WorkoutTrackerAPP.Enumerators;

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

        private List<ExerciseDTO> _filteredSource = new();



        // Filter collections
        public ObservableCollection<FilterOption> AvailableEquipment { get; } = new();
        public ObservableCollection<FilterOption> AvailableLevels { get; } = new();
        public ObservableCollection<FilterOption> AvailableCategories { get; } = new();
        public ObservableCollection<FilterOption> AvailablePrimaryMuscles { get; } = new();
        public ObservableCollection<FilterOption> AvailableSecondaryMuscles { get; } = new();

        public ExerciseLibraryViewModel(IExercises exercises)
        {
            _exercises = exercises;

            // Initialize filters first
            InitializeFilters();

            // Set initial state AFTER filters are ready
            IsFilterPanelVisible = false;

            ApplyFilter();

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

        private async void ApplyFilter()
        {
            // Capture current selections
            var searchText = SearchText;
            var selectedEquipment = AvailableEquipment.Where(f => f.IsSelected).Select(f => f.Name).ToHashSet();
            var selectedLevels = AvailableLevels.Where(f => f.IsSelected).Select(f => f.Name).ToHashSet();
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

            var itemsToLoad = _filteredSource
                                .Skip(_currentPage * PageSize)
                                .Take(PageSize)
                                .ToList();

            foreach (var item in itemsToLoad)
            {
                FilteredExercises.Add(item);
                //Debug.WriteLine($"{item.Name}");
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
        

        private void InitializeFilters()
        {
            // Clear first (important if re-initialized)
            AvailableEquipment.Clear();
            AvailableLevels.Clear();
            AvailableCategories.Clear();
            AvailablePrimaryMuscles.Clear();
            AvailableSecondaryMuscles.Clear();

            var exercises = _exercises.Exercises;

            var equipment = exercises
                .Select(e => e.Equipment)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(e => e)
                .Select(e => new FilterOption { Name = e, Category = EFilterCategory.Equipment });

            var levels = exercises
                .Select(e => e.Level)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct()
                .OrderBy(l => l)
                .Select(l => new FilterOption { Name = l, Category = EFilterCategory.Level });

            var categories = exercises
                .Select(e => e.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .OrderBy(c => c)
                .Select(c => new FilterOption { Name = c, Category = EFilterCategory.Category });

            var primaryMuscles = exercises
                .SelectMany(e => e.PrimaryMuscles ?? Enumerable.Empty<string>())
                .Distinct()
                .OrderBy(m => m)
                .Select(m => new FilterOption { Name = m, Category = EFilterCategory.PrimaryMuscle });

            var secondaryMuscles = exercises
                .SelectMany(e => e.SecondaryMuscles ?? Enumerable.Empty<string>())
                .Distinct()
                .OrderBy(m => m)
                .Select(m => new FilterOption { Name = m, Category = EFilterCategory.SecondaryMuscle });

            foreach (var item in equipment) AvailableEquipment.Add(item);
            foreach (var item in levels) AvailableLevels.Add(item);
            foreach (var item in categories) AvailableCategories.Add(item);
            foreach (var item in primaryMuscles) AvailablePrimaryMuscles.Add(item);
            foreach (var item in secondaryMuscles) AvailableSecondaryMuscles.Add(item);

            foreach (var option in AvailableEquipment
                .Concat(AvailableLevels)
                .Concat(AvailableCategories)
                .Concat(AvailablePrimaryMuscles)
                .Concat(AvailableSecondaryMuscles))
            {
                option.PropertyChanged -= OnFilterOptionChanged;
                option.PropertyChanged += OnFilterOptionChanged;
            }
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
                option.PropertyChanged -= OnFilterOptionChanged;

            foreach (var option in AllFilters())
                option.IsSelected = false;

            foreach (var option in AllFilters())
                option.PropertyChanged += OnFilterOptionChanged;

            ApplyFilter();
        }

        private IEnumerable<FilterOption> AllFilters() =>
            AvailableEquipment
                .Concat(AvailableLevels)
                .Concat(AvailableCategories)
                .Concat(AvailablePrimaryMuscles)
                .Concat(AvailableSecondaryMuscles);

    }


}
