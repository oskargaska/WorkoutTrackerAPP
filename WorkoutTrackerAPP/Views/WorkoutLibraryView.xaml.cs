using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views;

public partial class WorkoutLibraryView : ContentPage
{
	public WorkoutLibraryView(IWorkouts workouts, WorkoutLibraryViewModel workoutLibraryViewModel)
	{
		InitializeComponent();
		BindingContext = workoutLibraryViewModel;
	}
}