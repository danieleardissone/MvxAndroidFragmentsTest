using Android.Runtime;
using Android.Views;
using Android.OS;
using Android.Widget;
using MvxAndroidFragmentsTest.Droid.Views.Fragments;
using MvxAndroidFragmentsTest.Core.ViewModels;
using MvvmCross.Droid.Shared.Attributes;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.FullFragging.Fragments;

namespace MvxAndroidFragmentsTest.Droid.Views.Fragments
{
    //[MvxFragment((typeof(BaseViewModel)), Resource.Id.content_frame)]
    [MvxFragment((typeof(FragmentsHostViewModel)), Resource.Id.content_frame)] //, true)]
    [Register("mvxandroidfragmentstest.droid.views.fragments.SettingsFragment")]
	public class SettingsFragment : BaseNavigationFragment<SettingsViewModel>
	{
        protected override int FragmentId => Resource.Layout.SettingsView;

        protected new SettingsViewModel ViewModel
		{
			get {
				return base.ViewModel as SettingsViewModel;
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var ignore = base.OnCreateView(inflater, container, savedInstanceState);
			var view = this.BindingInflate(Resource.Layout.SettingsView, container, false);

			HasOptionsMenu = true;

			SetTitle(Resource.String.settings_title,Resource.Drawable.ic_toolbar_settings);
			HasBottomBar (true);

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.menu_top_detail, menu);
			//inflater.Inflate (Resource.Menu.menu_top_logout, menu);

			base.OnCreateOptionsMenu (menu, inflater);
		}

		public override void OnPause ()
		{
			base.OnPause ();
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) 
			{
			case Resource.Id.cancel:
				{
					ViewModel.DismissCommand.Execute ();
					return true;
				}

			case Resource.Id.done:
				{
					ViewModel.DoConfirmCommand.Execute ();
					return true;
				}
			}
			return base.OnOptionsItemSelected (item);
		}
	}
}
