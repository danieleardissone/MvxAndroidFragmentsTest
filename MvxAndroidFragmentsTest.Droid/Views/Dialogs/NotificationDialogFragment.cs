using System;
using Android.Content;
using Android.App;
using Android.OS;
using Android.Views;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Plugins.Messenger;
using MolloOfficina.Core.Messages;
using MolloOfficina.Core.ViewModels;
using MvvmCross.Binding.Droid.BindingContext;

namespace MolloOfficina.Droid.Views.Dialogs
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

		/*private IList<Notification> _notificationList;
		public IList<Notification> NotificationList {
			get {
				return _notificationList;
			}
			set {
				_notificationList = value;

				HandleNotificationListChanged();
			}
		}*/

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
			#region Binding


			/*this.DelayBind (() => {
				var set = this.CreateBindingSet<NotificationDialogFragment, ODLListViewModel> ();
				set.Bind (this).For (v => v.NotificationList).To (vm => vm.NotificationList);
				set.Apply ();
			});*/
			#endregion

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
			//if (_notificationList == null)
				//return;

			//_list = this.View.FindViewById<MvxListView> (Resource.Id.NotificationList);

			Type t = ViewModel.GetType ();

			if (t == typeof(ODLListViewModel)) {
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
			}

			/*_adapter = new MvxAdapter ((ODLListView)Context, (MvxAndroidBindingContext)BindingContext);

			((ODLListView)Context).RunOnUiThread (() => {
				_list.Adapter = _adapter;
			});*/
		}
	}
}


