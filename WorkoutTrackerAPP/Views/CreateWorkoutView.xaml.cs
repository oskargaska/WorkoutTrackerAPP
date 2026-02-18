using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views;

public partial class CreateWorkoutView : ContentPage
{
	public CreateWorkoutView(IExercises exercises, IWorkouts workouts, CreateWorkoutViewModel createWorkoutViewModel)
	{
		InitializeComponent();
		BindingContext = createWorkoutViewModel;
	}
}