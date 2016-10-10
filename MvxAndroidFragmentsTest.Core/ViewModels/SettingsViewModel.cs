using System;
using System.Net;
using Plugin.Permissions.Abstractions;
using MolloOfficina.Core.Services;
using MolloOfficina.Core.Data;
using MvvmCross.Core.ViewModels;
using MolloOfficina.Core.Exceptions;
using MolloOfficina.Core.Resources;
using MolloOfficina.Core.Helpers;

namespace MolloOfficina.Core.ViewModels
{
	public class SettingsViewModel : BaseNavigationViewModel
    {
		//private readonly MvxSubscriptionToken _subscrNotifToken;

		private ISettingsService _settingsSvc = null;
		//private readonly IMvxFileStore _fileStore;

		public SettingsViewModel (ISettingsService settingsService) : base () //, IProfileOnlineService profileOnlineService) : base()
		{
			_settingsSvc = settingsService;
		}

		public override void Start ()
		{
			base.Start ();
			RefreshViewStateCommand.Execute ();
		}

		#region Properties to bind
		/*private string _syncServer = string.Empty;
		public string SyncServer
		{ 
			get { return _syncServer; }
			set
			{
				if (!SetProperty (ref _syncServer, value))
					return;
			}
		}*/

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
		public IMvxCommand DoConfirmCommand { get { return new MvxCommand (DoConfirm); } }

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

		private async void DoConfirm()
		{
			try
			{
                if (!SettingsData.IsModified)
                    ShowMessageBox(CoreResources.alert_generic_title, CoreResources.alert_notmodified_msg); //, null);
				else
				{
					/*Validate();
					if (HasErrors)
					{
						ShowMessageBox(CoreResources.alert_generic_title, CoreResources.alert_validation_error_msg, null); 
						return;
					}*/

					ServiceReturn retValue = null;

                    //retValue = await ShowLoadingForLongOperationAsync(async () => await _settingsSvc.SaveAsync(SettingsData));

                    await ShowLoadingForLongOperationAsync<bool>(async () =>
                    {
                        bool res = await CheckRunTimePermissions(Permission.Storage);

                        if (res)
                        {
                            retValue = await _settingsSvc.SaveAsync(SettingsData);
                            return true;
                        }
                        else
                            return false;
                    });

                    if (retValue == null)
                        ShowMessageBox("Permessi mancanti", "Per procedere è necessario disporre dei permessi di lettura/scrittura sul device."); //, null);

                    else if (retValue.Parameter)
					{
						Close(this);
					}
					else
						RaiseGeneralFailEvent(new MolloOfficinaException(
							retValue.ErrorMessages[0]));
					//CoreResources.err_save));
				}
			}
			catch (MolloOfficinaOfflineException ex) {
				RaiseGeneralFailEvent (ex);
			}
			catch (WebException ex) {
				RaiseGeneralFailEvent (ex);
			}
			catch (Exception ex) {
				RaiseGeneralFailEvent(ex);
			}
		}

		public override IMvxCommand RefreshViewStateCommand { get { return new MvxCommand (DoRefresh); } }
		public async void DoRefresh()
		{
			try
			{
				await ShowLoadingForLongOperationAsync(async () => SettingsData = await _settingsSvc.GetSettingsDataAsync());

                /*await ShowLoadingForLongOperationAsync<bool>(async () =>
                {
                    bool res = await CheckRunTimePermissions();

                    if (res)
                    {
                        SettingsData = await _settingsSvc.GetSettingsDataAsync();
                        return true;
                    }
                    else
                        return false;
                });*/

                SetModified(false);
				//await ShowLoadingForLongOperationAsync<bool>(async () => MenuItemList = await _menuSvc.GetAllByUserAsync(Settings.LastUsername));
				//LastSync = "Ultima sincronizzazione: " + SyncLog.ReadTimeLog(ZumeroSync.NAVSERVICE_TITLE);
			}
			catch (MolloOfficinaOfflineException ex) {
				RaiseGeneralFailEvent (ex);
			}
			catch (WebException ex) {
				RaiseGeneralFailEvent (ex);
			}
			catch (Exception ex)
			{
				RaiseGeneralFailEvent(ex);
			}
		}
		#endregion

		private void SetModified(bool value)
		{
			SettingsData.IsModified = value;
		}
    }
}
