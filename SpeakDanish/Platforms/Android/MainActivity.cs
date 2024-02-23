using Android.App;
using Android.Content.PM;
using Android.OS;

namespace SpeakDanish
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            Instance = this;

            base.OnCreate(savedInstanceState);
        }

    }
}
