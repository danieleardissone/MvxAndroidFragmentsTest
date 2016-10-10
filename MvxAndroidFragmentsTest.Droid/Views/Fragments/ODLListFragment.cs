using System;
using Android.Runtime;
using Android.Views;
using Android.OS;
using Android.Widget;
using MolloOfficina.Core.ViewModels;
using MvvmCross.Droid.Shared.Attributes;
using MolloOfficina.Core.Enums;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Binding.BindingContext;

namespace MolloOfficina.Droid.Views.Fragments
{
    //[MvxFragment((typeof(ODLListViewModel)), Resource.Id.content_frame, true)]
    [Register("molloofficina.droid.views.fragments.ODLListFragment")]
	public class ODLListFragment : BaseNavigationFragment<ODLListViewModel>
	{
        protected override int FragmentId => Resource.Layout.ODLListContainer;

        //private Android.Support.V7.Widget.Toolbar _topBar = null;
        /*private bool _orderByName = true;
		public bool OrderByName {
			get {
				return _orderByName;
			}
			set {
				if (_orderByName == value)
					return;

				_orderByName = value;
				HandleODLListOrderChanged ();
			}
		}*/

        private bool _sortChanging = false;

		private WOHListSortEnum _sortType = WOHListSortEnum.None;
		public WOHListSortEnum SortType {
			get {
				return _sortType;
			}
			set {
				//if (_sortType != value) 
					//ViewModel.ResetSortCommand.Execute ();

					//return;

				if (_sortType == value && !_sortChanging)
					return;

				_sortType = value;
				HandleODLListOrderChanged (_sortType);
			}
		}

		protected new ODLListViewModel ViewModel
		{
			get {
				return base.ViewModel as ODLListViewModel;
			}
		}

		/*private IList<CoreODLItemViewModel> _odlList = null;
		public IList<CoreODLItemViewModel> ODLList {
			get {
				return _odlList;
			}
			set 
			{
				if (_odlList == value)
					return;
				
				_odlList = value;
				HandleODLListOrderChanged ();
			}
		}*/

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var ignore = base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(FragmentId, null); //, container, false);

			view.FindViewById<LinearLayout>(Resource.Id.linearLayout1).Click += OrderListByName;
			view.FindViewById<LinearLayout>(Resource.Id.linearLayout2).Click += OrderListByDate;
			view.FindViewById<LinearLayout>(Resource.Id.linearLayout3).Click += OrderListByCustomer;

			var myActivity = (ODLListView) this.Activity;
			myActivity.sw.Focusable = false;
			if (myActivity.sw!=null)
			{
				myActivity.sw.QueryTextChange += (sender, e) => {
					string query = myActivity.sw.Query;
					//if (query != null) {
						ViewModel.SearchText=query;
						ViewModel.DoSearchCommand.Execute ();
						//if (string.IsNullOrEmpty(query))
							//ViewModel.DoSearchCommand.Execute ();					
					//}
				};

				myActivity.sw.QueryTextSubmit += (sender, e) => {
					string query = myActivity.sw.Query;
		


					if (query != null) {
						ViewModel.DoSearchCommand.Execute ();
					}

				};
			}

			//ViewModel.SearchText = String.Empty;

			HasOptionsMenu = true;

			SetTitle(Resource.String.odllist_title);
			HasBottomBar (true);

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			#region Binding
			var set = this.CreateBindingSet<ODLListFragment, ODLListViewModel>();
			set.Bind (this).For(o => o.SortType).To (vm => vm.SortType);
			set.Apply ();
			#endregion

			LoadListLayout ();
			//HandleODLListOrderChanged (_sortType);
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.menu_list_sync, menu);
			//inflater.Inflate (Resource.Menu.menu_top_logout, menu);

			base.OnCreateOptionsMenu (menu, inflater);
		}

		private void HandleODLListOrderChanged(WOHListSortEnum sortType)
		{
			if (sortType == WOHListSortEnum.None)
				return;
			
			UpdateTabButtons (sortType);

			if (ViewModel.SortType != _sortType)
				ViewModel.SortType = _sortType;
			
			ViewModel.DoChangeOrderCommand.Execute ();

			//LoadListLayout ();

			/*if (_orderByNumber) {
				LoadListLayout ();
				LoadList (_orderByNumber);
			}*/
		}

		public override void OnPause ()
		{
			base.OnPause ();

			//QUESTO MI SERVE PERCHE' NON AVENDO LA CACHE, AL RESUME DEL FRAGMENT VENGONO RICARICATE LE ODL.
			//LA PROPERTY SORTTYPE, PER LANCIARE IL RICARICAMENTO, NON DEVE ESSERE UGUALE ALL'ORDINAMENTO CORRENTE.
			//NONe E' IL VALORE DELL'ENUMERATION DA UTILIZZARE PER RESETTARE LA PROPERTY.
			SortType = WOHListSortEnum.None;
		}

		void OrderListByDate (object sender, EventArgs e)
		{
			if (SortType.Equals (WOHListSortEnum.Date)) {
				_sortChanging = true;
				ViewModel.SortDateDesc = !ViewModel.SortDateDesc;
			}

			SortType = WOHListSortEnum.Date;
			_sortChanging = false;
			//UpdateTabButtons (SortType);
			/*LoadListLayout ();
			LoadList (_orderByNumber);*/
		}

		void OrderListByName (object sender, EventArgs e)
		{
			if (SortType.Equals (WOHListSortEnum.No)) {
				_sortChanging = true;
				ViewModel.SortNoDesc = !ViewModel.SortNoDesc;
			}

			SortType = WOHListSortEnum.No;
			_sortChanging = false;
			//if (!firstNoSelection)
			//InvertSameOrder (firstNoSelection);

			//UpdateTabButtons (SortType);
			/*LoadListLayout ();
			LoadList (_orderByNumber);*/
		}

		void OrderListByCustomer (object sender, EventArgs e)
		{
			//ViewModel.SortTypeAndDirection [WOHListSortEnum.Customer] = !(ViewModel.SortTypeAndDirection [WOHListSortEnum.Customer]);
			if (SortType.Equals (WOHListSortEnum.Customer)) {
				_sortChanging = true;
				ViewModel.SortCustomerDesc = !ViewModel.SortCustomerDesc;
			}

			SortType = WOHListSortEnum.Customer;
			_sortChanging = false;
			//if (!firstCustomerSelection)
			//InvertSameSortType (firstCustomerSelection);

			//UpdateTabButtons (SortType);
			/*LoadListLayout ();
			LoadList (_orderByNumber);*/
		}

		//private void InvertSameSortType

		private void UpdateTabButtons(WOHListSortEnum sortType)
		{
			switch (sortType) 
			{
			case WOHListSortEnum.No:
				ActivateButtons (true, false, false);
				break;
			case WOHListSortEnum.Date:
				ActivateButtons (false, true, false);
				break;
			case WOHListSortEnum.Customer:
				ActivateButtons (false, false, true);
				break;
			}

			//View tabButtons = View.FindViewById<Button>(Resource.Id.linearLayoutTabButtons);
		}

		private void ActivateButtons(bool activeNo, bool activeDate, bool activeCustomer)
		{
			View btnOrderNumber = View.FindViewById<LinearLayout>(Resource.Id.linearLayout1);
			View btnOrderDate = View.FindViewById<LinearLayout>(Resource.Id.linearLayout2);
			View btnOrderCustomer = View.FindViewById<LinearLayout>(Resource.Id.linearLayout3);

			btnOrderNumber.Activated = activeNo;
			btnOrderDate.Activated = activeDate;
			btnOrderCustomer.Activated = activeCustomer;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			/*case Resource.Id.refresh:
				{

					ViewModel.DoChangeOrderFromWSCommand.Execute ();
					return true;
				}*/
			case Resource.Id.sync:
				{
					ViewModel.SyncCommand.Execute();
					return true;
				}
			}
			return base.OnOptionsItemSelected (item);
		}

		private void LoadListLayout()
		{
			FrameLayout frameLayout = this.View.FindViewById<FrameLayout> (Resource.Id.content_frame_inner);
			frameLayout.RemoveAllViews ();
			this.BindingInflate (Resource.Layout.ODLListView, frameLayout, true);
			//var list = this.View.FindViewById<MvxListView> (Resource.Id.ODLMvxListView);
		}

		public override void PerformCustomAction ()
		{
			base.PerformCustomAction ();
			ViewModel.ApplyFiltersCommand.Execute ();
		}

		/*private void LoadList(bool orderByNumber)
		{
			ViewModel.OrderByName = orderByNumber;
			ViewModel.DoChangeOrderCommand.Execute ();
		}*/
	}
}
