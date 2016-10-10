using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Views;

namespace MvxAndroidFragmentsTest.Droid
{
    [Activity(
        Label = "@string/splash_title"
        , MainLauncher = true
        , Icon = "@drawable/ic_launcher_main"
        , Theme = "@style/Theme.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }
    }
}
