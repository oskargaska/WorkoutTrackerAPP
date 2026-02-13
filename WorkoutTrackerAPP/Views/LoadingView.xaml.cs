using System.Threading.Tasks;
using WorkoutTrackerAPP.Interfaces;

namespace WorkoutTrackerAPP.Views;

public partial class LoadingView : ContentPage
{
	public LoadingView(IExercises exercises)
	{
		InitializeComponent();
	}

    
}