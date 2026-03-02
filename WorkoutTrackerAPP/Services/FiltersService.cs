using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using WorkoutTrackerAPP.Enumerators;
using WorkoutTrackerAPP.Filters;
using WorkoutTrackerAPP.Interfaces;

internal class FiltersService : IFilters
{
    private readonly IDatabase _database;
    private bool _isLoaded = false;

    // Strongly-typed collections for each category
    public ObservableCollection<FilterOption> Categories { get; private set; } = new();
    public ObservableCollection<FilterOption> Forces { get; private set; } = new();
    public ObservableCollection<FilterOption> Levels { get; private set; } = new();
    public ObservableCollection<FilterOption> Equipment { get; private set; } = new();
    public ObservableCollection<FilterOption> PrimaryMuscles { get; private set; } = new();
    public ObservableCollection<FilterOption> SecondaryMuscles { get; private set; } = new();


    public FiltersService(IDatabase database)
    {
        _database = database;
    }

    public async Task LoadFromDatabaseAsync()
    {
        if (_isLoaded) return;
        _isLoaded = true;

        // Load lists from database
        var muscles = await _database.GetMusclesAsync();
        var categories = await _database.GetCategoriesAsync();
        var forces = await _database.GetForcesAsync();
        var levels = await _database.GetLevelsAsync();
        var equipment = await _database.GetEquipmentAsync();

        // Populate ObservableCollections
        Categories = new ObservableCollection<FilterOption>(categories);
        Forces = new ObservableCollection<FilterOption>(forces);
        Levels = new ObservableCollection<FilterOption>(levels);
        Equipment = new ObservableCollection<FilterOption>(equipment);

        // PrimaryMuscles
        PrimaryMuscles = new ObservableCollection<FilterOption>(
            muscles.Select(m => new FilterOption
            {
                Name = m.Name,
                Category = EFilterCategory.Muscle
            })
        );

        // SecondaryMuscles
        SecondaryMuscles = new ObservableCollection<FilterOption>(
            muscles.Select(m => new FilterOption
            {
                Name = m.Name,
                Category = EFilterCategory.Muscle
            })
        );
    }

    public void ResetAllFilters()
    {
        void ResetCollection(ObservableCollection<FilterOption> collection)
        {
            if (collection.Count == 0) return;

            foreach (var option in collection)
            {
                option.IsSelected = false; // if you want selections cleared too
            }
        }

        ResetCollection(Categories);
        ResetCollection(Forces);
        ResetCollection(Levels);
        ResetCollection(Equipment);
        ResetCollection(PrimaryMuscles);
        ResetCollection(SecondaryMuscles);
    }

}