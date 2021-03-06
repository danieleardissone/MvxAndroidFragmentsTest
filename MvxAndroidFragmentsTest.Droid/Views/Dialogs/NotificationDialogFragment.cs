﻿using System;
using Android.Content;
using Android.App;
using Android.OS;
using Android.Views;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Plugins.Messenger;
using MvxAndroidFragmentsTest.Core.Messages;
using MvxAndroidFragmentsTest.Core.ViewModels;
using MvvmCross.Binding.Droid.BindingContext;

namespace MvxAndroidFragmentsTest.Droid.Views.Dialogs
{
	public class NotificationDialogFragment : BaseDialogFragment
	{
		private MvxAdapter _adapter = null;
		private MvxListView _list = null;

		private readonly MvxSubscriptionToken _subscrNotifToken;

		public NotificationDialogFragment (Context context) : base(context)
		{
			Context = context;

			_subscrNotifToken = base.Subscribe<TotNewNotificationsChangedMessage>(HandleNotificationListChanged);
		}

		protected new BaseViewModel ViewModel
		{
			get { return base.ViewModel as BaseViewModel; }
			//set { base.ViewModel = value; }
		}

		public override Dialog OnCreateDialog (Bundle savedState)
		{
			base.EnsureBindingContextSet (savedState);

			var view = this.BindingInflate (Resource.Layout.NotificationList, null, false); //Resource.Layout.

			Dialog markerPopUpWindow = new Dialog (Context);
			markerPopUpWindow.RequestWindowFeature ((int)WindowFeatures.NoTitle);
			markerPopUpWindow.SetContentView (view);
			markerPopUpWindow.SetCancelable (true);

			markerPopUpWindow.FindViewById<View> (Resource.Id.btnDismiss).Click += (_, e) => {
				Dismiss ();
			};

			_list = markerPopUpWindow.FindViewById<MvxListView> (Resource.Id.NotificationList);

			return markerPopUpWindow;
		}

		public override void OnDismiss (IDialogInterface dialog)
		{
			base.OnDismiss (dialog);

			if (DismissListener != null)
				DismissListener.OnDismiss(dialog);
		}

		private void HandleNotificationListChanged(TotNewNotificationsChangedMessage sender)
		{
			Type t = ViewModel.GetType ();

			/*if (t == typeof(ODLListViewModel)) {
				_adapter = new MvxAdapter ((ODLListView)Context, (MvxAndroidBindingContext)BindingContext);

				((ODLListView)Context).RunOnUiThread (() => {
					_list.Adapter = _adapter;
				});
			}
			else if (t == typeof(MenuViewModel)) {
				_adapter = new MvxAdapter ((MenuView)Context, (MvxAndroidBindingContext)BindingContext);

				((MenuView)Context).RunOnUiThread (() => {
					_list.Adapter = _adapter;
				});
			}*/
		}
	}
}


