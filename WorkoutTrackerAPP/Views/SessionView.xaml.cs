using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views;

public partial class SessionView : ContentPage
{
	
	public SessionView(SessionViewModel sessionViewModel)
	{
		InitializeComponent();
		BindingContext = sessionViewModel;
	}
}