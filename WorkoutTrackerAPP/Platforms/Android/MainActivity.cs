using Android.App;
using Android.Content.PM;
using Android.OS;

namespace WorkoutTrackerAPP
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public override bool DispatchTouchEvent(Android.Views.MotionEvent ev)
        {
            if (ev.Action == Android.Views.MotionEventActions.Down)
            {
                var imm = (Android.Views.InputMethods.InputMethodManager)GetSystemService(InputMethodService);
                imm.HideSoftInputFromWindow(CurrentFocus?.WindowToken, 0);
                CurrentFocus?.ClearFocus();
            }
            return base.DispatchTouchEvent(ev);
        }
    }
}
