using System.Diagnostics;
using System.Threading.Tasks;
using WorkoutTrackerAPP.Interfaces;
using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views
{ 
    public partial class LoadingView : ContentPage
    {
        private readonly LoadingViewModel _viewModel;
	    public LoadingView(IExercises exercises, LoadingViewModel loadingViewModel)
        {
            InitializeComponent();
            BindingContext = loadingViewModel;
            _viewModel = loadingViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("OnAppearing aufgerufen");
            await _viewModel.InitializeAsync();
        }
    }
}

    


