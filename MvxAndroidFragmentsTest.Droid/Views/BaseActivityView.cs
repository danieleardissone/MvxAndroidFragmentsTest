using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using FluentValidation;
using FluentValidation.Results;
using Android.Util;
using Android.Support.V4.App;
using System.Net;
using Android.Content.PM;
using MvvmCross.Platform;
using MvxAndroidFragmentsTest.Core.Entities;
using MvxAndroidFragmentsTest.Core.Exceptions;
using Android.Content.Res;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Core.ViewModels;
using MvxAndroidFragmentsTest.Core.ViewModels;
using MvxAndroidFragmentsTest.Droid.Views.Dialogs;

namespace MvxAndroidFragmentsTest.Droid.Views
{		
	public class BaseActivityView<TViewModel> : MvxCachingFragmentCompatActivity<TViewModel> where TViewModel : class, IMvxViewModel //FragmentActivityWithActionBar, ICustomPresenterListener
	{
		private const string TAG = "BaseActivityView";
		private const string NOTIFICATION_FRAGMENT_TAG = "NotificationFragment";

		private bool _hasBottomBar = true;
		public bool HasBottomBar
		{
			get { return _hasBottomBar; }
			set 
			{ 
				_hasBottomBar = value; 

				Android.Support.V7.Widget.Toolbar bottomBar = FindViewById<Android.Support.V7.Widget.Toolbar> (Resource.Id.bottom_toolbar);

				if (!_hasBottomBar)
					bottomBar.Visibility = ViewStates.Gone;
				else
					bottomBar.Visibility = ViewStates.Visible;
			}
		}

		protected BaseActivityView()
		{

		}

		protected new BaseViewModel ViewModel {
			get { return base.ViewModel as BaseViewModel; }
		}

		#region ActionBar Stuff
		public void SetCenteredTitle(int aResId)
		{
			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.top_toolbar);
			if (toolbar == null)
				return;
			var titleView = toolbar.FindViewById<TextView>(Resource.Id.top_title);

			if (titleView == null)
				return;
			titleView.SetText(aResId);
		}

		public void SetCenteredTitle(int aResId, int aIconId)
		{
			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.top_toolbar);
			if (toolbar == null)
				return;
			var titleView = toolbar.FindViewById<TextView>(Resource.Id.top_title);

			if (titleView == null)
				return;
			titleView.SetText(aResId);
			titleView.SetCompoundDrawablesRelativeWithIntrinsicBounds (aIconId, 0, 0, 0);
		}

		protected void SetupActionBar(int aTitle)
		{
			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.top_toolbar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetHomeButtonEnabled(false);
			SupportActionBar.SetDisplayShowTitleEnabled(false);
			SupportActionBar.SetDisplayUseLogoEnabled(true);
			SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_back_logo);
			SetCenteredTitle(aTitle);
		}

		public override bool OnSupportNavigateUp()
		{
			Android.Content.Intent upIntent = NavUtils.GetParentActivityIntent(this);
			if (upIntent == null)
			{
				NavUtils.NavigateUpFromSameTask(this);
				return true;
			}
			if (NavUtils.ShouldUpRecreateTask(this, upIntent))
			{
				// this activity is not part of this app's task
				global::Android.Support.V4.App.TaskStackBuilder.Create(this).AddNextIntentWithParentStack(upIntent).StartActivities();
			}
			else
			{
				// this activity is part of this app's task
				NavUtils.NavigateUpTo(this, upIntent);
			}
			return true;
		}
		#endregion

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			//Log.Debug(TAG, "OnCreate()");

			Android.Content.Intent intent = Intent;
			Bundle extraBundle = intent.Extras;
			if (bundle == null)
			{
				/*				
                We can't add events here, because during the orientation change, the View gets replaced and
                the ViewModel events will be notified to the old view... (SupportFragmentManager.IsDestroyed = true)
                BaseVM.PropertyChanged += BaseVM_PropertyChanged;
                */
			}

			//if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1) {	// IsDestroyed has been introduced since API Level 17
			if (SupportFragmentManager.IsDestroyed)
			{
				//Log.Error(TAG, "Activity has been already destroyed");
				return;
			}
		}

		protected virtual void HandleGeneralFailure (object sender, EventArgs<Exception> e)
		{
			if (e.Value is MvxAndroidFragmentsTestOfflineException || e.Value is WebException)
			{
				ShowAlertDialogFragment (e.Value.Message, MessageTypeEnum.Offline);
				return;
			}
			ShowAlertDialogFragment (e.Value.Message, MessageTypeEnum.Error);
		}

		protected void ShowAlertDialogFragment (int aMessage, MessageTypeEnum aType)
		{
			ShowAlertDialogFragment (Resources.GetString (aMessage), aType);
		}

		protected void ShowAlertDialogFragment (string aMessage, MessageTypeEnum aType)
		{
			ShowAlertDialogFragment (aMessage, Resources.GetString (Resource.String.common_warning), Resource.Drawable.ic_dialog_warning_shadow);
		}

		protected void ShowAlertDialogFragment(string aMessage, string aTitle, int aIcon)
		{
			if (SupportFragmentManager.IsDestroyed)
			{
				Log.Error(TAG, "Activity has been already destroyed");
				return;
			}
			AlertDialogFragment frag = AlertDialogFragment.NewInstance(aMessage, aTitle, aIcon);
			SupportFragmentManager.BeginTransaction().Add(frag, NOTIFICATION_FRAGMENT_TAG + frag.GetHashCode()).Commit();
		}

		private Android.Support.V4.App.Fragment FindFragment(string aTag)
		{			if (SupportFragmentManager.Fragments == null)
			{
				return null;
			}

			foreach (Android.Support.V4.App.Fragment fragment in SupportFragmentManager.Fragments)
			{
				if (fragment != null)
				{
					if (fragment.Tag == aTag)
						return fragment;
				}
			}
			return null;
		}

		protected override void OnResume()
		{
			base.OnResume();

			if (ViewModel != null) {
				ViewModel.GeneralFailure += HandleGeneralFailure;
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			if (ViewModel != null) {
				ViewModel.GeneralFailure -= HandleGeneralFailure;
			}

			Log.Debug (TAG, "OnPause()");
		}

		protected virtual void HandleValidationError (object sender, EventArgs<ValidationException> e)
		{
			String msg = String.Empty;
			foreach (ValidationFailure failure in e.Value.Errors) {
				if (msg != String.Empty)
					msg += "\n";
				msg += failure.ErrorMessage;
			}
			Toast toast = Toast.MakeText (this, msg, ToastLength.Long);
			toast.SetGravity (GravityFlags.CenterHorizontal | GravityFlags.CenterVertical, 0, 0);
			toast.Show ();
		}
    }
}


