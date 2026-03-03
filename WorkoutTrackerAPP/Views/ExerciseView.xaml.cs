using Syncfusion.Maui.Toolkit.Carousel;
using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views;

public partial class ExerciseView : ContentPage
{
    private readonly ExerciseViewModel _viewModel;
    public ExerciseView(ExerciseViewModel exerciseViewModel)
	{
		InitializeComponent();
	    _viewModel = exerciseViewModel;
		BindingContext =  _viewModel;
	}

    public void SetExercise(string exerciseId)
    {
        _ = _viewModel.LoadExerciseAsync(exerciseId);
    }
}