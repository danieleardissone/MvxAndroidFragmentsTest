using System;
using System.Net;
using Plugin.Permissions.Abstractions;
using MvxAndroidFragmentsTest.Core.Services;
using MvxAndroidFragmentsTest.Core.Data;
using MvvmCross.Core.ViewModels;
using MvxAndroidFragmentsTest.Core.Exceptions;
using MvxAndroidFragmentsTest.Core.Resources;
using MvxAndroidFragmentsTest.Core.Helpers;

namespace MolloOfficina.Core.ViewModels
{
	public class SettingsViewModel : BaseNavigationViewModel
    {
		public SettingsViewModel () : base ()
		{

		}

		public override void Start ()
		{
			base.Start ();
			RefreshViewStateCommand.Execute ();
		}

		#region Properties to bind
		private SettingsData _settingsData;
		public SettingsData SettingsData {
			get { return _settingsData; }
			set { 
				if (_settingsData == value)
					return;

				_settingsData = value;
				RaiseAllPropertiesChanged ();
			}
		}
		#endregion

		#region Commands
		public IMvxCommand DismissCommand { get { return new MvxCommand (DoDismiss); } }
		//public IMvxCommand DoConfirmCommand { get { return new MvxCommand (DoConfirm); } }

		private void DoDismiss()
		{
			try
			{
				Close(this);
			}
			catch (Exception ex) {
				RaiseGeneralFailEvent (ex);
			}
		}
		#endregion
    }
}
