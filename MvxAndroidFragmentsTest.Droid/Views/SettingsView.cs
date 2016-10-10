using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Content.PM;
using Android.Util;
using MolloOfficina.Core.ViewModels;
using MolloOfficina.Droid.Views.Fragments;
using MolloOfficina.Droid.Helpers;

namespace MolloOfficina.Droid.Views
{
	[Activity(Label = "@string/settings_title", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden, Name = "molloofficina.droid.views.SettingsView")]			
	public class SettingsView : BaseActivityView<SettingsViewModel> //BaseActivityView, IMvxFragmentHost //ViewTreeObserver.IOnGlobalLayoutListener
	{
		private View _mainView = null;
		private Android.Support.V7.Widget.Toolbar _bottomBar = null;
		private Android.Support.V7.Widget.Toolbar _topBar = null;
		public Android.Widget.SearchView sw = null;

		protected new BaseNavigationViewModel ViewModel
		{
			get { return base.ViewModel as BaseNavigationViewModel; }
			//set { base.ViewModel = value; }
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);
			//SetContentView(Resource.Layout.ODLListView);
			SetContentView(Resource.Layout.BaseNavigationView);

			_mainView = FindViewById<View> (Resource.Id.main_root);

			SetupActionBar(Resource.String.settings_title);
			//SupportActionBar.SetHomeAsUpIndicator (Resource.Drawable.ic_menu_noback_logo);
			SupportActionBar.SetHomeAsUpIndicator (Resource.Drawable.ic_menu_backtomenu);
			_topBar = FindViewById<Android.Support.V7.Widget.Toolbar> (Resource.Id.top_toolbar);

			_bottomBar = FindViewById<Android.Support.V7.Widget.Toolbar> (Resource.Id.bottom_toolbar);
			_bottomBar.MenuItemClick += HandleBottomBarClick;

			//RegisterFragments (bundle);

			//if (bundle == null)
				//ShowFragment (Tags.SETTINGS, Resource.Id.content_frame, bundle);
			this.Window.SetSoftInputMode (SoftInput.StateHidden);
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();

			//UnregisterFragment ();
		}

		/*public void OnGlobalLayout()
		{
			// Mostra / Nasconde la bottombar in accordo con lo spazio di schermo disponibile
			Rect r = new Rect();
			_mainView.GetWindowVisibleDisplayFrame (r);

			//int minPx = DisplayHelper
		}*/

		protected override void OnResume ()
		{
			base.OnResume ();

			ViewModel.GeneralFailure -= HandleGeneralFailure;
			//_mainView.ViewTreeObserver.AddOnGlobalLayoutListener (this);
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			//_mainView.ViewTreeObserver.RemoveOnGlobalLayoutListener (this);
		}

		/*public override void OnFragmentChanging (Android.Support.V4.App.Fragment aNewFragment, string tag, Android.Support.V4.App.FragmentTransaction transaction)
		{
			base.OnFragmentChanging (aNewFragment, tag, transaction);

			if (transaction != null) 
			{
				transaction.AddToBackStack (null);
			}

			sw = _topBar.FindViewById<Android.Widget.SearchView> (Resource.Id.search_view);
			sw.Visibility = ViewStates.Gone;

			if (tag == Tags.SETTINGS) {
				IMvxFragmentView fragView = aNewFragment as IMvxFragmentView;
				fragView.ViewModel = ViewModel;
				//sw = _topBar.FindViewById<Android.Widget.SearchView> (Resource.Id.search_view);
				//sw.Visibility = ViewStates.Visible;
				
				var item = _topBar.FindViewById (Resource.Id.top_title);
				item.Visibility = ViewStates.Visible;
				//item.Visibility = ViewStates.Gone;

			}else {
				var item = _topBar.FindViewById (Resource.Id.top_title);
				item.Visibility = ViewStates.Visible;
				//sw = _topBar.FindViewById<Android.Widget.SearchView> (Resource.Id.search_view);
				//sw.Visibility = ViewStates.Gone;
			}

			//UpdateBottomBar (tag);
			UpdateTopBar (tag);
		}*/

		private void UpdateTopBar(string tag)
		{
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_back);

            switch (tag) 
			{
			case Tags.SETTINGS:
					SupportActionBar.SetDisplayHomeAsUpEnabled (true);
				SupportActionBar.SetHomeAsUpIndicator (Resource.Drawable.ic_menu_back);
					//SupportActionBar.SetHomeAsUpIndicator (Resource.Drawable.ic_menu_noback_logo);
					break;
				default:
					SupportActionBar.SetDisplayHomeAsUpEnabled (true);
					SupportActionBar.SetHomeAsUpIndicator (Resource.Drawable.ic_menu_back);
					break;
			}
		}

		private void UpdateBottomBar(string tag)
		{
			int resId = 0;

			switch (tag) 
			{
			case Tags.SETTINGS:
					//resId = Resource.Menu.menu_bottom_filters;
					//resId = Resource.Menu.menu_bottom_notifications;
					resId = Resource.Menu.menu_bottom_logout;	
					break;
				/*case Tags.ODL_FILTERS:
					resId = Resource.Menu.menu_bottom_filters;
					break;
				case Tags.ODL_DETAIL:
					resId=Resource.Menu.menu_bottom_logout;
					break;
				case Tags.MATERIAL_LIST:
					//resId = Resource.Menu.menu_bottom_add;
					break;
				case Tags.COST_LIST:
					//resId = Resource.Menu.menu_bottom_add;
					break;
				case Tags.HOUR_LIST:
					//resId = Resource.Menu.menu_bottom_add;
					break;
					case Tags.ACTIVITY_LIST:
					//resId = Resource.Menu.menu_bottom_add;
					break;*/
				default:
					resId = Resource.Menu.menu_bottom_logout;
					break;
			}
				
			if (resId != 0) {
				_bottomBar.Menu.Clear ();
				MenuInflater.Inflate (resId, _bottomBar.Menu);

				View btn = _bottomBar.FindViewById (Resource.Id.logout);

				if (btn != null) {
					btn.Selected = (tag == Tags.LOGOUT);
				}
			}

		}

		/*private void RegisterFragments(Bundle bundle)
		{
			RegisterFragment<SettingsFragment, SettingsViewModel> (Tags.SETTINGS, bundle);
			//RegisterFragment<PermitAddEditFragment, PermitAddEditViewModel> (Tags.PERMIT_ADD_EDIT, bundle);
		}

		private void UnregisterFragment()
		{
			var customPresenter = Mvx.Resolve<IMvxFragmentsPresenter> ();
			customPresenter.UnRegisterViewModelAtHost<SettingsViewModel> ();
			//customPresenter.UnRegisterViewModelAtHost<PermitAddEditViewModel> ();
		}

		protected void RegisterFragment<TFragment, TViewModel>(string tag, Bundle args, IMvxViewModel viewModel = null)
			where TFragment : IMvxFragmentView
			where TViewModel : IMvxViewModel
		{
			var customPresenter = Mvx.Resolve<IMvxFragmentsPresenter> ();
			customPresenter.RegisterViewModelAtHost<TViewModel> (this);
			RegisterFragment<TFragment, TViewModel> (tag);
		}

		bool IMvxFragmentHost.Show(MvxViewModelRequest request, Bundle bundle)
		{
			ViewGroup content_frame = _mainView.FindViewById<ViewGroup> (Resource.Id.content_frame);
			content_frame.RemoveAllViews ();

			return false;
		}*/

		void HandleBottomBarClick(object sender, Android.Support.V7.Widget.Toolbar.MenuItemClickEventArgs e)
		{
			if (sw != null) {
				sw.ClearFocus ();
			
			}

			switch (e.Item.ItemId) 
			{
				case Resource.Id.logout:
					ViewModel.LogoutCommand.Execute ();
					break;
			/*case Resource.Id.notifications:
				HandleNotificationClick ();
					break;
				case Resource.Id.filters:
					HandleStickyClick ();
					break;
				//default:*/
				
			}
		}

		private void HandleStickyClick()
		{
			BaseFragment<SettingsViewModel> frag = SupportFragmentManager.FindFragmentById (Resource.Id.content_frame) as BaseFragment<SettingsViewModel>;
			frag.PerformCustomAction ();
		}

		/*private Type TopFragmentType
		{
			get 
			{
				if (SupportFragmentManager.Fragments == null)
					return typeof(SettingsFragment);

				Android.Support.V4.App.Fragment res = null;

				foreach(Android.Support.V4.App.Fragment fragment in SupportFragmentManager.Fragments)
				{
					if (fragment == null) 
					{
						Log.Debug ("Fragment, ", "NULL");
					} 
					else 
					{
						res = fragment;
						Log.Debug ("Fragment, ", fragment.ToString());
					}
				}

				if (res != null)
					return res.GetType ();

				return typeof(PermitListFragment);
			}
		}*/

		public override bool OnSupportNavigateUp ()
		{
			//Type topFrag = TopFragmentType;
			//if (topFrag == typeof(SettingsFragment)) ||
		    //if (topFrag == typeof(ODLFiltersViewModel))
				//return false;

			OnBackPressed ();

			return true;
		}

		/*public override bool OnSupportNavigateUp ()
		{
			Type topFrag = TopFragmentType;
			if (topFrag != typeof(SettingsFragment)) { //||
				ChangePresentation (new ClearNavBackStackHint ());
			}

			OnBackPressed ();
			return true;
		}*/

		public override void OnBackPressed ()
		{
            base.OnBackPressed();

			/*Type topFrag = TopFragmentType;
			if (topFrag == typeof(SettingsFragment))
			{
				RemoveLastFragmentFromBackStack ();
				base.OnBackPressed ();
				//return;
			}

			RemoveLastFragmentFromBackStack ();*/
		}
	}
}

