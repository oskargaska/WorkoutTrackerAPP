using System.Diagnostics;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views;

public partial class ActiveWorkoutView : ContentPage
{
    private readonly ActiveWorkoutViewModel _viewModel;
    public ActiveWorkoutView(ActiveWorkoutViewModel activeWorkoutViewModel)
    {
        InitializeComponent();
        _viewModel = activeWorkoutViewModel;
        BindingContext = activeWorkoutViewModel;
    }

    protected override bool OnBackButtonPressed()
    {
        _viewModel.GoBackCommand.Execute(null);
        return true;
    }

}