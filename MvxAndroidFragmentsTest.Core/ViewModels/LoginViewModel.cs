using FluentValidation;
using System;
using System.Net;
using Version.Plugin;
using System.IO;
using MvvmCross.Plugins.File;
using MolloOfficina.Core.Resources;
using MolloOfficina.Core.Helpers;
using MolloOfficina.Core.Exceptions;
using MvvmCross.Core.ViewModels;
using MolloOfficina.Core.Constants;
using Plugin.Permissions.Abstractions;
using MolloOfficina.Core.Services;

namespace MolloOfficina.Core.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{
		private IProfileService _profileSvc = null;
		private readonly IMvxFileStore _fileStore;
        //private readonly IMvxPermissions _permissions;
        //private readonly IMvxResourceLoader _resourceloader;
        //private IProfileOnlineService _profileOnlineSvc = null;

        public LoginViewModel (IProfileService profileService, IMvxFileStore fileStore) : base() //, IMvxResourceLoader resourceloader) : base () //, IProfileOnlineService profileOnlineService) : base()
		{
			_profileSvc = profileService;
			_fileStore = fileStore;

			Validator = new LoginViewModelValidator ();
		}

		#region Properties to bind
		private string _username = string.Empty;
		public string Username
		{ 
			get { return _username; }
			set
			{
				if (!SetProperty (ref _username, value))
					return;
			}
			//set { _username = value; RaisePropertyChanged(() => Username); }
		}

		private string _password = string.Empty;
		public string Password
		{ 
			get { return _password; }
			set
			{
				if (!SetProperty (ref _password, value))
					return;
			}
		}

		public string Disclaimer
		{ 
			get { 
				string[] versionNumber = CrossVersion.Current.Version.Split ('.');
				return "Powered by Novus SI srl - v. " + versionNumber[0] + "." + versionNumber[1]; 
			}

		}
		#endregion

		#region Commands
		public IMvxCommand LoginCommand { get { return new MvxCommand (DoLogin, CanLogin); } }
		public IMvxCommand SettingsCommand { get { return new MvxCommand (DoSettings); } }
		#endregion

		private void DoSettings()
		{
			ShowViewModelAndClearBackStack<SettingsViewModel> ();
		}

		private bool CanLogin()
		{
			return true;
		}

		private async void DoLogin()
		{
			/*if (IsBusy)
				return;

			IsBusy = true;*/

			try
			{
				if (String.IsNullOrEmpty(Settings.SyncServer))
				{
                    ShowMessageBox(CoreResources.alert_generic_title, CoreResources.alert_syncservernotsetted_msg); //, null);
					return;
				}

				Validate();
				if (HasErrors)
				{
					//GetErrors(
					//ShowMessageBox("Errore di validazione", 
					return;
				}
				//bool success = false;

				// IL CAMBIO UTENTE DEVE ESSERE SEMPRE ONLINE PERCHE' SERVE FARE UNA SINCRONIZZAZIONE DEI DATI
				if (Username.Trim() != _profileSvc.LastUsername.Trim())
					CheckDeviceConnectivity();

				#region Controllo che sia installato il servizio di sincronizzazione e poi effettuo il login
				bool checkSyncServiceInstalled = await ShowLoadingForLongOperationAsync<bool>(async () => 
					{
						bool checkSyncService = await _profileSvc.CheckSyncServiceAsync();

						if (checkSyncService)
						{
							//CHECK FILE DI FINE SYNC
							if (!_fileStore.Exists(System.IO.Path.Combine(ZumeroSync.DB_STANDARD_PATH, ZumeroSync.CONFIRM_FIRST_SYNC)))
							{
								//ShowMessageBox("Attendere", CoreResources.err_syncservicenotlaunched, null);
								RaiseGeneralFailEvent(new MolloOfficinaException(
									CoreResources.err_syncservicenotlaunched));
								return false;
							}
							else 
							{
								byte[] output = null;
								_fileStore.TryReadBinaryFile(System.IO.Path.Combine(ZumeroSync.DB_STANDARD_PATH, ZumeroSync.CONFIRM_FIRST_SYNC), out output);

								if (_fileStore.Exists(System.IO.Path.Combine(ZumeroSync.DB_STANDARD_PATH, ZumeroSync.CONFIRM_FIRST_SYNC)) && output.Length == 0)
								{
									//ShowMessageBox("Attendere", CoreResources.err_syncservicenotcompleted, null);

									RaiseGeneralFailEvent(new MolloOfficinaException(
										CoreResources.err_syncservicenotcompleted));

									return false;
								}
							}

							return true;
							//return await _profileSvc.LoginAsync(Username, Password);
						}
						else
						{
							RaiseGeneralFailEvent(new MolloOfficinaException(
								CoreResources.err_syncservice));
							return false;
						}
					});
				#endregion

				if (!checkSyncServiceInstalled)
					return;

                bool res = await CheckRunTimePermissions(Permission.Phone);

                if (res)
                {
                    bool success = await ShowLoadingForLongOperationAsync<bool>(async () => await _profileSvc.LoginAsync(Username, Password));

                    //bool success = await _profileSvc.LoginAsync(Username, Password);

                    //CHECK FILE DI FINE SYNC
                    /*if (!_fileStore.Exists(System.IO.Path.Combine(ZumeroSync.DB_STANDARD_PATH, ZumeroSync.CONFIRM_FIRST_SYNC)))
                    {
                        ShowMessageBox("Attendere", "E' in corso la prima sincronizzazione dell'applicazione. Si prega di attendere.", null);
                        return;
                    }*/

                    if (success)
                    {
                        _profileSvc.LastUsername = Username;
                        _profileSvc.LastPassword = Password;

                        ShowViewModelAndClearBackStack<MenuViewModel>();
                    }
                    else
                    {
                        RaiseGeneralFailEvent(new MolloOfficinaException(
                            CoreResources.err_login));

                        //currentUser = _profileOnlineSvc.Find(Username, Password);

                        //if (!_profileSvc.Insert(currentUser))
                        //	RaiseGeneralFailEvent(new NavServiceAppException(
                        //		"Autenticazione fallita. Controllare le credenziali e riprovare."));
                    }
                }
			}
			catch (MolloOfficinaOfflineException ex) {
				RaiseGeneralFailEvent (ex);
			}
			catch (WebException ex) {
				RaiseGeneralFailEvent (ex);
			}
			catch (Exception ex) {
				RaiseGeneralFailEvent (ex);
			}
			/*finally {
				IsBusy = false;
			}*/
		}


		protected override void InitFromBundle (IMvxBundle parameters)
		{
			Username = _profileSvc.LastUsername;
			Password = _profileSvc.LastPassword;

			#if DEVELOP
			Username = Constants.Development.USERNAME;
			Password = Constants.Development.PASSWORD;
			_sessionSvc.HideWalkthrough = false;
			_sessionSvc.ShowTooltipDashboard = true;
			_sessionSvc.ShowTooltipProducts = true;
			#endif
		}

		/*private bool CheckNavServicePath(string path)
		{
			var pathComposed = _fileStore.PathCombine("NavService", path + ".pdf");

			return _fileStore.Exists (pathComposed);
		}

		private string GetNavServicePath(string path)
		{
			return _fileStore.PathCombine("NavService", path + ".pdf");
		}*/

		private byte[] ReadFully(Stream input)
		{
			byte[] buffer = new byte[16*1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}

		/*private void CopyDataBase() 
		{
			var pathComposed = _fileStore.PathCombine(ZumeroSync.DB_STANDARD_PATH, ZumeroSync.DB_NAME);

			bool res = _fileStore.Exists (pathComposed);

			_fileStore.DeleteFile (pathComposed);

			// Check if your DB has already been extracted.
			if (!_fileStore.Exists (pathComposed))
			{
				//_fileStore.OpenWrite (pathComposed);

				try
				{
					_resourceloader.GetResourceStream(ZumeroSync.DB_NAME, (stream) =>
						{
							byte[] streamByte = ReadFully(stream);

							_fileStore.WriteFile(pathComposed, streamByte);
						});
				}
				catch (Exception ex)
				{
					RaiseGeneralFailEvent (ex);
				}
			}
		}*/
	}

	public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
	{
		public LoginViewModelValidator()
		{
			RuleFor (x => x.Username).NotEmpty ().WithMessage ("Lo Username non può essere vuoto.");
			RuleFor (x => x.Password).NotEmpty ().WithMessage ("La password non può essere vuota.");
		}
	}
}
