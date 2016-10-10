using System;
using System.Net;
using System.Collections.Generic;
using MvvmCross.Platform;
using MvvmCross.Plugins.File;
using MvxAndroidFragmentsTest.Core.Resources;
using MvxAndroidFragmentsTest.Core.Constants;
using MvxAndroidFragmentsTest.Core.Helpers;
using MvvmCross.Core.ViewModels;
using MvxAndroidFragmentsTest.Core.Exceptions;
using MvxAndroidFragmentsTest.Core.Services;
using MvxAndroidFragmentsTest.Core.Data;

namespace MvxAndroidFragmentsTest.Core.ViewModels
{
	public class BaseNavigationViewModel : BaseViewModel
	{
		private IProfileService _profileSvc = null;
		private INotificationService _notificationSvc = null;
		private IMenuService _menuSvc = null;

		public BaseNavigationViewModel ()
		{

		}

		public BaseNavigationViewModel (IProfileService profileService, INotificationService notificationService)
		{
			_profileSvc = Mvx.Resolve<IProfileService> ();
			//_notificationSvc = Mvx.Resolve<INotificationService> ();
		}

		#region Commands
		//public virtual IMvxCommand SyncCommand { get { return null; } }

		public IMvxCommand LogoutCommand { get { return new MvxCommand (DoLogout); } }

		private void DoLogout()
		{
			//Action<bool> onConfirm = new Action<bool> (Confirm);

			try
			{
				ShowConfirmMessageBox(CoreResources.confirm_logout_title, String.Empty, new Action<bool> (ConfirmLogout));
				//ShowViewModelAndClearBackStack<LoginViewModel> ();
			}
			catch (Exception ex) {
				RaiseGeneralFailEvent(new MolloOfficinaException(
					CoreResources.confirm_logout_error));
			}
		}

		public IMvxCommand RefreshNotificationsCommand { get { return new MvxCommand (DoRefreshNotifications); } }

		private MvxCommand<Notification> _notificationSelectedCommand;
		public IMvxCommand NotificationSelectedCommand
		{
			get
			{
				_notificationSelectedCommand = _notificationSelectedCommand ?? new MvxCommand<Notification>(MarkNotificationAsRead);
				return _notificationSelectedCommand;
			}
		}
		//public IMvxCommand ShowNotificationsCommand { get { return new MvxCommand (DoShowNotifications); } }

		public virtual IMvxCommand SyncCommand { get { return new MvxCommand (DoSync); } }
		private async void DoSync()
		{
			_menuSvc = Mvx.Resolve<IMenuService> ();

			try
			{
				CheckDeviceConnectivity();

				bool success = await ShowLoadingForLongOperationAsync<bool>(async () => 
					{
						await _menuSvc.SyncAsync(Settings.LastUsername, Settings.LastPassword);
						LastSync = UpdateLastSync();

						return true;
					}
				);
			}
			catch (MvxAndroidFragmentsTestOfflineException ex) {
				RaiseGeneralFailEvent (ex);
			}
			catch (WebException ex) {
				RaiseGeneralFailEvent (ex);
			}
			catch (Exception ex) {
				RaiseGeneralFailEvent(ex);
			}
		}
		#endregion

		protected virtual string UpdateLastSync()
		{
			Customer cust = _menuSvc.GetCustomer();
			return cust != null ? cust.BusinessName + " - Ultima sincronizzazione: " + SyncLog.ReadTimeLog(ZumeroSync.NOVUSMOBILE_TITLE) : "Ultima sincronizzazione: " + SyncLog.ReadTimeLog(ZumeroSync.NOVUSMOBILE_TITLE);
			//return "Ultima sincronizzazione " + SyncLog.ReadTimeLog (ZumeroSync.NAVSERVICE_TITLE);
		}

		private void RemoveDB()
		{
			IMvxFileStore _fileStore = Mvx.Resolve<IMvxFileStore>();

			var pathComposed = _fileStore.PathCombine(ZumeroSync.DB_STANDARD_PATH, ZumeroSync.DB_NAME);
			if (_fileStore.Exists (pathComposed))
				_fileStore.DeleteFile (pathComposed);
		}

		private async void ConfirmLogout(bool flag)
		{
			if (flag) 
			{
				try
				{
					_profileSvc = Mvx.Resolve<IProfileService> ();

					bool success = await ShowLoadingForLongOperationAsync<bool>(async () => await _profileSvc.LogoutAsync());

					if (success)
						ShowViewModelAndClearBackStack<LoginViewModel> ();
					else
						RaiseGeneralFailEvent(new MolloOfficinaException(
							CoreResources.err_logout));
				}
				catch (MvxAndroidFragmentsTestOfflineException ex) {
					RaiseGeneralFailEvent (ex);
				}
				catch (WebException ex) {
					RaiseGeneralFailEvent (ex);
				}
				catch (Exception ex) {
					RaiseGeneralFailEvent (ex);
				}
				
			}
		}
	}
}

