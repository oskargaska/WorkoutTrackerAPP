namespace WorkoutTrackerAPP.Views;

using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.ViewModels;

public partial class ExerciseLibraryView : ContentPage
{

	public ExerciseLibraryView(ExerciseLibraryViewModel exerciseLibraryViewModel)
	{
		InitializeComponent();
		BindingContext = exerciseLibraryViewModel;

	}

	
}