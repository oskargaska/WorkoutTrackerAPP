using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views;

public partial class CreateWorkoutView : ContentPage
{
	public CreateWorkoutView(CreateWorkoutViewModel createWorkoutViewModel)
	{
		InitializeComponent();
		BindingContext = createWorkoutViewModel;

        
    }
}