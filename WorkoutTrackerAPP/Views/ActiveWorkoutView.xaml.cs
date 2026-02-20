using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views;

public partial class ActiveWorkoutView : ContentPage
{
	public ActiveWorkoutView(IWorkouts workouts, ActiveWorkoutViewModel activeWorkoutViewModel)
	{
		InitializeComponent();
		BindingContext = activeWorkoutViewModel;
	}
}