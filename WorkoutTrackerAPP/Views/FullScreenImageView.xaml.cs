
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Input;

namespace WorkoutTrackerAPP.Views;

public partial class FullScreenImageView : ContentPage
{
    public string Source { get; set; }  
	public FullScreenImageView(string source)
	{
        InitializeComponent();
        
        Source = source;
        Debug.WriteLine($"{Source}");
        BindingContext = this;


    }
    public ICommand CloseCommand => new Command(async () => await Shell.Current.Navigation.PopAsync());

}