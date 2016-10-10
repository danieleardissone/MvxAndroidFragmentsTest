using Android.Views;
using Android.OS;
using MolloOfficina.Droid.Views;
using MolloOfficina.Core.ViewModels;
using MvvmCross.Core.ViewModels;

namespace MolloOfficina.Droid.Views.Fragments
{
	public abstract class BaseNavigationFragment<TViewModel> : BaseFragment<TViewModel> where TViewModel : class, IMvxViewModel
    {
		public BaseNavigationFragment ()
		{
            
		}

		protected void SetTitle(int aResId)
		{
            // TODO: Change title in a different way...
            Views.BaseActivityView<TViewModel> activ = Activity as Views.BaseActivityView<TViewModel>;
			activ.SetCenteredTitle(aResId);
		}

		protected void SetTitle(int aResId, int aIconId)
		{
			// TODO: Change title in a different way...
			BaseActivityView<TViewModel> activ = Activity as Views.BaseActivityView<TViewModel>;
			activ.SetCenteredTitle(aResId, aIconId);
		}

		protected void HasBottomBar(bool visibility)
		{
			BaseActivityView<TViewModel> activ = Activity as BaseActivityView<TViewModel>;
			activ.HasBottomBar = visibility;
		}

		protected BaseNavigationViewModel ViewModelNavigation
		{
			get { return base.ViewModel as BaseNavigationViewModel; }
			//set { base.ViewModel = value; }
		}

		protected new BaseViewModel ViewModel
		{
			get { return base.ViewModel as BaseViewModel; }
			//set { base.ViewModel = value; }
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			/*InputMethodManager manager = (InputMethodManager) GetSystemService(InputMethodService);
			manager.HideSoftInputFromWindow(etMyEditText.WindowToken, 0);*/
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
		}

		/*public override void OnAttach(Android.App.Activity activity)
		{
			base.OnAttach(activity);
		}*/

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return base.OnCreateView(inflater, container, savedInstanceState);
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
		}

		public override void OnDestroyView()
		{
			base.OnDestroyView();
		}

		/*public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.menu_top_logout, menu);
			base.OnCreateOptionsMenu (menu, inflater);
		}*/

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) 
			{
				case Resource.Id.logout:
				{
					ViewModelNavigation.LogoutCommand.Execute ();
					return true;
				}

				/*case Resource.Id.done:
					{
						//ViewModelNavigation.LogoutCommand.Execute ();
						return true;
					}*/
			}

			return base.OnOptionsItemSelected (item);
		}
	}
}

