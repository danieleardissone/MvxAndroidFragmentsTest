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
using Android.Telephony;
using Android.Content.PM;
using Plugin.Permissions;
using MvvmCross.Platform;
using MolloOfficina.Core.Entities;
using MolloOfficina.Core.Exceptions;
using MolloOfficina.Core.Enums;
using Android.Content.Res;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Core.ViewModels;
using MolloOfficina.Core.ViewModels;
using MolloOfficina.Droid.Views.Dialogs;

namespace MolloOfficina.Droid.Views
{		
	public class BaseActivityView<TViewModel> : MvxCachingFragmentCompatActivity<TViewModel> where TViewModel : class, IMvxViewModel //FragmentActivityWithActionBar, ICustomPresenterListener
	{
		private const string TAG = "BaseActivityView";
		private const string NOTIFICATION_FRAGMENT_TAG = "NotificationFragment";

		private TelephonyManager _telephonyManager;

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
			/*CustomPresenter presenter = Mvx.Resolve<CustomPresenter> ();
			if (presenter != null)
				presenter.SetListener (this);*/
		}

		/*public void ChangePresentation(MvxPresentationHint hint)
		{
			if (hint is MvxClosePresentationHint) {
				RemoveLastFragmentFromBackStack ();
			}

			if (hint is ClearNavBackStackHint) 
			{
				while (SupportFragmentManager.BackStackEntryCount > 0) 
				{
					RemoveLastFragmentFromBackStack ();
				}
			}
		}*/

		protected new BaseViewModel ViewModel {
			get { return base.ViewModel as BaseViewModel; }
			//set { base.ViewModel = value; }
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

		/*protected override void AttachBaseContext(Context context)
		{
			base.AttachBaseContext(new CalligraphyContextWrapper(context));
		}*/

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

		#region ToolBar stuff
		/*protected void SetupBottomBar(ref Toolbar _bottomBar)
		{
			Toolbar bottomBar = FindViewById<Toolbar> (Resource.Id.bottom_toolbar);

			if (!HasBottomBar) {
				bottomBar.Visibility = false;
				return;
			}
			
			//bottomBar.MenuItemClick += HandleBottomBarClick;
		}*/
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

			// QUI CONTROLLO SE IL DEVICE PUO' O MENO EFFETTUARE CHIAMATE
			/*_telephonyManager = (TelephonyManager)GetSystemService(Context.TelephonyService);
			if (_telephonyManager.IsSmsCapable) 
			{
				//IS PHONE 
			}*/
		}

		protected virtual void HandleGeneralFailure (object sender, EventArgs<Exception> e)
		{
			if (e.Value is MolloOfficinaOfflineException || e.Value is WebException)
			{
				//new OfflineDialogFragment(Application).Show(SupportFragmentManager, "OfflineDialogFragment");
				//new OfflineDialogFragment(CallingActivity).Show(SupportFragmentManager, "OfflineDialogFragment");
				ShowAlertDialogFragment (e.Value.Message, MessageTypeEnum.Offline);
				return;
			}
			ShowAlertDialogFragment (e.Value.Message, MessageTypeEnum.Error);
		}

		/*protected virtual void HandleWebCallError (object sender, EventArgs<ResponseCodeDto> e)
        {
            ShowAlertDialogFragment (e.Value.Description, MessageTypeEnum.Warning);
        }*/

		protected void ShowAlertDialogFragment (int aMessage, MessageTypeEnum aType)
		{
			ShowAlertDialogFragment (Resources.GetString (aMessage), aType);
		}

		protected void ShowAlertDialogFragment (string aMessage, MessageTypeEnum aType)
		{
			switch (aType) {
			case MessageTypeEnum.Information:
				ShowAlertDialogFragment (aMessage, Resources.GetString (Resource.String.common_information), Resource.Drawable.ic_dialog_information_shadow); //Resource.Drawable.tmp_ic_dialog_info);
				break;
			case MessageTypeEnum.Warning:
				ShowAlertDialogFragment (aMessage, Resources.GetString (Resource.String.common_warning), Resource.Drawable.ic_dialog_warning_shadow); //Resource.Drawable.tmp_ic_dialog_warning);
				break;
			case MessageTypeEnum.Error:
				ShowAlertDialogFragment (aMessage, Resources.GetString (Resource.String.common_error), Resource.Drawable.ic_dialog_error_shadow); //Resource.Drawable.tmp_ic_dialog_error);
				break;
			case MessageTypeEnum.Offline:
				ShowAlertDialogFragment (aMessage, Resources.GetString (Resource.String.common_offline), Resource.Drawable.ic_dialog_error_shadow); //Resource.Drawable.tmp_ic_dialog_error);
				break;
			default:
				break;
			}
		}

		protected void ShowAlertDialogFragment(string aMessage, string aTitle, int aIcon)
		{
			//if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1) {	// IsDestroyed has been introduced since API Level 17
			if (SupportFragmentManager.IsDestroyed)
			{
				Log.Error(TAG, "Activity has been already destroyed");
				return;
			}
			//} /**/
			AlertDialogFragment frag = AlertDialogFragment.NewInstance(aMessage, aTitle, aIcon);
			SupportFragmentManager.BeginTransaction().Add(frag, NOTIFICATION_FRAGMENT_TAG + frag.GetHashCode()).Commit();
		}

		//DA TESTARE ABILITANDO CALLIGRAPHY
		/*protected override void AttachBaseContext(Context context)
		{
			base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
		}*/

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

		/*protected override bool ShouldReplaceFragment(int contentId, string currentTag, string replacementTag)
		{
			// If the fragment already exists in the backstack, remove all the fragments until the parent is reached...
			Android.Support.V4.App.Fragment frag = null;
			while ((frag = FindFragment(replacementTag)) != null)
			{
				SupportFragmentManager.PopBackStackImmediate();
			}
			return true;
		}*/

		protected override void OnResume()
		{
			base.OnResume();

			if (ViewModel != null) {
				ViewModel.GeneralFailure += HandleGeneralFailure;
				//ViewModel.ErrorsChanged += HandleErrorsChanged;
				//            ViewModel- += HandleValidationError;
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			/* Check: Removing Event Handlers in Activities
        http://developer.xamarin.com/guides/cross-platform/deployment,_testing,_and_metrics/memory_perf_best_practices/
            */

			//          ViewModel.PropertyChanged -= HandlePropertyChanged;
			if (ViewModel != null) {
				ViewModel.GeneralFailure -= HandleGeneralFailure;
				//ViewModel.ErrorsChanged -= HandleErrorsChanged;
			}

			// TODO: Temporary handles the invalid form data, will be moved to ConcreteView
			//Hockey.StopTrackingUsage (this);
			//Hockey.UnregisterManagers ();
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //MvxPermissions.RequestPermissionsAsync(PLugin);
        }
    }
}


