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
using MvxAndroidFragmentsTest.Core.Data;

namespace MvxAndroidFragmentsTest.Core.ViewModels
{
	public class BaseNavigationViewModel : BaseViewModel
	{
		public BaseNavigationViewModel ()
		{

		}

		public BaseNavigationViewModel ()
		{
		}

		#region Commands
		public IMvxCommand LogoutCommand { get { return new MvxCommand (DoLogout); } }

		private void DoLogout()
		{
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
		#endregion

		private async void ConfirmLogout(bool flag)
		{
			if (flag) 
			{
				try
				{
					ShowViewModelAndClearBackStack<LoginViewModel> ();

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

