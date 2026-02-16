using WorkoutTrackerAPP.ViewModels;

namespace WorkoutTrackerAPP.Views
{
    public partial class MainView : ContentPage
    {

        public MainView(MainViewModel mainViewModel)
        {

            InitializeComponent();
            BindingContext = mainViewModel;
        }

        
    }
}
