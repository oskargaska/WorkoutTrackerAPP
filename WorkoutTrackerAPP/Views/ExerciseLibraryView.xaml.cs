namespace WorkoutTrackerAPP.Views;

using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.ViewModels;

public partial class ExerciseLibraryView : ContentPage
{

	public ExerciseLibraryView(IExercises exercises, ExerciseLibraryViewModel exerciseLibraryViewModel)
	{
		InitializeComponent();
		BindingContext = exerciseLibraryViewModel;

	}

	
}