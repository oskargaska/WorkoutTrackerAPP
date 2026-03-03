using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views;

public partial class HistoryView : ContentPage
{
	public HistoryView(HistoryViewModel historyViewModel)
	{
		InitializeComponent();
		BindingContext = historyViewModel;
	}
}