using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using MvxAndroidFragmentsTest.Core.ViewModels;
using Android.Views.InputMethods;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using MvvmCross.Droid.Support.V7.AppCompat;

namespace MvxAndroidFragmentsTest.Droid.Views
{
    [Activity(
        LaunchMode = LaunchMode.SingleTop,
        Name = "mvxandroidfragmentstest.droid.views.FragmentsHostView"
        )]
    public class FragmentsHostView : BaseActivityView<FragmentsHostViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.FragmentsHostView);
        }

        public void HideSoftKeyboard()
        {
            if (CurrentFocus == null) return;

            InputMethodManager inputMethodManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);

            CurrentFocus.ClearFocus();
        }
    }
}
