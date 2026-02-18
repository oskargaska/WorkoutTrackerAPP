using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views;

public partial class ExercisePickerView : ContentPage
{
	public ExercisePickerView(IExercises exercises, ExercisePickerViewModel exercisePickerViewModel)
	{
		InitializeComponent();
		BindingContext = exercisePickerViewModel;
	}
}