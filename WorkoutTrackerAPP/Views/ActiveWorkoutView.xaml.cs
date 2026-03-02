using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views;

public partial class ActiveWorkoutView : ContentPage
{
	public ActiveWorkoutView(ActiveWorkoutViewModel activeWorkoutViewModel)
	{
		InitializeComponent();
		BindingContext = activeWorkoutViewModel;
	}
}