using System;
using System.Net;
using System.Collections.Generic;
using MvvmCross.Plugins.Messenger;
using MolloOfficina.Core.Services;
using MvvmCross.Plugins.File;
using MolloOfficina.Core.Messages;
using MolloOfficina.Core.Helpers;
using MolloOfficina.Core.Constants;
using MvvmCross.Core.ViewModels;
using MolloOfficina.Core.Exceptions;
using MolloOfficina.Core.Data;

namespace MolloOfficina.Core.ViewModels
{
	public class MenuViewModel : BaseNavigationViewModel
    {
		private readonly MvxSubscriptionToken _subscrNotifToken;

		private IMenuService _menuSvc = null;
		private INotificationService _notificationSvc = null;

		private ISignService _signSvc = null;
		private readonly IMvxFileStore _fileStore;

		public MenuViewModel (INotificationService notificationService, IMenuService menuService, IMvxFileStore fileStore, ISignService signService) : base () //, IProfileOnlineService profileOnlineService) : base()
		{
			_notificationSvc = notificationService;
			_menuSvc = menuService;
			//_profileOnlineSvc = profileOnlineService;

			_fileStore = fileStore;
			_signSvc = signService;

			_subscrNotifToken = base.Subscribe<TotNewNotificationsChangedMessage>(OnNotificationsChanged);
		}

		private void OnNotificationsChanged(TotNewNotificationsChangedMessage aSrc)
		{
			RaisePropertyChanged(() => TotNewNotifications);

			//L'AGGIORNAMENTO DELLA LISTA DI NOTIFICHE VIENE FATTO TRAMITE IL DIALOG IN ASCOLTO SUL MESSAGE DELLE TOTNOTIFICATIONS.
			//NON RIUSCIVO A SETTARE IL BINDING CONTEXT PER CREARE UN BINDING SET NEL DIALOG.

			//RefreshNotificationsCommand.Execute ();
			//RaisePropertyChanged(() => NotificationList);
			//RefreshNotificationsCommand.Execute ();
		}

		public override void Start ()
		{
			base.Start ();
			RefreshViewStateCommand.Execute ();

			if (NotificationList == null)
				RefreshNotificationsCommand.Execute();
		}

		#region Properties to bind
		public int TotNewNotifications
		{
			get { return _notificationSvc.TotNewNotifications; }
		}

		private IList<MenuItem> _menuItemList;
		public IList<MenuItem> MenuItemList {
			get {
				return _menuItemList;
			}
			private set {
				if (_menuItemList == value)
					return;

				_menuItemList = value;

				RaisePropertyChanged ();
			}
		}

		private Customer _customer = null;
		public Customer Customer {
			get { return _customer; }
			set { 
				if (_customer == value)
					return;

				_customer = value;
				RaiseAllPropertiesChanged ();
			}
		}

		/*private byte[] _logo = null;
		public byte[] Logo {
			get { return _logo; }
			set { 
				if (_logo == value)
					return;

				_logo = value;
				RaiseAllPropertiesChanged ();
			}
		}*/

		/*private string _lastSync;
		public string LastSync
		{ 
			get { 
				return _lastSync; //"Ultima sincronizzazione: " + SyncLog.ReadTimeLog(ZumeroSync.NAVSERVICE_TITLE); //+ versionNumber[0] + "." + versionNumber[1];
			}

			set {

				if (value == _lastSync)
					return;

				_lastSync = value;
				RaisePropertyChanged (() => LastSync);
			}
		}*/
		#endregion

		#region Commands
		public IMvxCommand DoShowWorkOrderListCommand { get { return new MvxCommand(() => { }); } } //ShowViewModel<ODLListViewModel> (); }); } }

		public override IMvxCommand RefreshViewStateCommand { get { return new MvxCommand (DoRefresh); } }
		public async void DoRefresh()
		{
			try
			{
				await ShowLoadingForLongOperationAsync<bool>(async () => 
					{
						MenuItemList = await _menuSvc.GetAllByUserAsync(Settings.LastUsername);
						 //"Ultima sincronizzazione: " + SyncLog.ReadTimeLog(ZumeroSync.NAVSERVICE_TITLE);
						Customer = await _menuSvc.GetCustomerAsync();

						LastSync = UpdateLastSync();

						//if (Customer != null)
							//Logo = Customer.Logo;

						return true;
					});

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

		private MvxCommand<MenuItem> _itemSelectedCommand;
		public IMvxCommand ItemSelectedCommand
		{
			get
			{
				_itemSelectedCommand = _itemSelectedCommand ?? new MvxCommand<MenuItem>(DoSelectItem);
				return _itemSelectedCommand;
			}
		}

		private void DoSelectItem(MenuItem item)
		{
			try
			{
				/*switch (item.Type)
				{
				case MenuItemEnum.WorkOrderHeaders:
					{
						ShowViewModel<ODLListViewModel> ();
						break;
					}
				case MenuItemEnum.Vacations:
					{
						//ShowMessageBox("Ferie / Permessi", "Work in progress...", null);
						ShowViewModel<VacationListViewModel> ();
						break;
					}
				case MenuItemEnum.Permits:
					{
						//ShowMessageBox("Permessi", "Work in progress...", null);
						ShowViewModel<PermitListViewModel> ();
						break;
					}
				case MenuItemEnum.HealthService:
					{
						//ShowMessageBox("Mutua", "Work in progress...", null);
						ShowViewModel<HealthServiceListViewModel> ();
						break;
					}
				case MenuItemEnum.GasStations:
					{
						//ShowMessageBox("Funzionalità non disponibile", "Funzionalità non disponibile in Novus Mobile.", null);
						ShowViewModel<GasStationAddEditViewModel> ();
						break;
					}
				case MenuItemEnum.TecnicalDocumentation:
					{
						//ShowMessageBox("Funzionalità non disponibile", "Funzionalità non disponibile in Novus Mobile.", null);
						ViewTechnicalDocumentation();
						break;
					}
				case MenuItemEnum.VehicleExitCard:
					{
						//ShowMessageBox("Funzionalità non disponibile", "Funzionalità non disponibile in Novus Mobile.", null);
						GenerateVehicleExitCardDocument();
						break;
					}
				case MenuItemEnum.WorkShopOrder:
					{
						//ShowMessageBox("Funzionalità non disponibile", "Funzionalità non disponibile in Novus Mobile.", null);
						//ShowViewModel<WorkShopOrderAddEditViewModel> ();

						GenerateWorkShopOrderDocument();
						break;
					}
				case MenuItemEnum.None:
					{
                            ShowMessageBox("Funzionalità non disponibile", "Funzionalità non disponibile in Novus Mobile."); //, null);
						break;
					}
				default:
					{
						//ShowMessageBox("Funzionalità non disponibile", "Funzionalità non disponibile in Novus Mobile.", null);
						break;
					}
				}*/
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

			//IsBusy = false;
		}

		#region PER DEMO - DA RIVEDERE
		private byte[] _workShopOrderDocument;
		public byte[] WorkShopOrderDocument {
			get {
				return _workShopOrderDocument;
			}
			set {
				_workShopOrderDocument = value;
				RaisePropertyChanged ();
			}
		}

		private byte[] _vehicleExitCardDocument;
		public byte[] VehicleExitCardDocument {
			get {
				return _vehicleExitCardDocument;
			}
			set {
				_vehicleExitCardDocument = value;
				RaisePropertyChanged ();
			}
		}

		private string _pdfPath = string.Empty;
		public string PDFPath {
			get {
				return _pdfPath;
			}
			set {
				_pdfPath = value;
				RaisePropertyChanged (() => PDFPath);
			}
		}

		private bool _openNovuSign;
		public bool OpenNovuSign {
			get {
				return _openNovuSign;
			}
			set {
				_openNovuSign = value;

				RaisePropertyChanged (() => OpenNovuSign);
			}
		}

		private bool _openNovusDocs;
		public bool OpenNovusDocs {
			get {
				return _openNovusDocs;
			}
			set {
				_openNovusDocs = value;

				RaisePropertyChanged (() => OpenNovusDocs);
			}
		}

		private async void ViewTechnicalDocumentation()
		{
			OpenNovusDocs = true;

			/*await ShowLoadingForLongOperationAsync<bool>(async () => 
				{
					//ItemTypeList = await _workOrderLineSvc.GetItemTypeAsync();
					WorkShopOrderDocument = await _signSvc.GetWorkShopOrderDocumentAsync(PDFPath);
				});*/
		}

		private async void GenerateWorkShopOrderDocument()
		{
			PDFPath = "OrdineOfficina";

			await ShowLoadingForLongOperationAsync<bool>(async () => 
				{
					//ItemTypeList = await _workOrderLineSvc.GetItemTypeAsync();
					WorkShopOrderDocument = await _signSvc.GetWorkShopOrderDocumentAsync(PDFPath);

					if (WorkShopOrderDocument != null) // && FileSigned.File.Data != null)
					{
						SaveFileToLocal (WorkShopOrderDocument, PDFPath);
					}
					return true;
				});
		}

		private async void GenerateVehicleExitCardDocument()
		{
			PDFPath = "SchedaUscitaMezzo";

			await ShowLoadingForLongOperationAsync<bool>(async () => 
				{
					//ItemTypeList = await _workOrderLineSvc.GetItemTypeAsync();
					VehicleExitCardDocument = await _signSvc.GetVehicleExitCardDocumentAsync(PDFPath);

					if (VehicleExitCardDocument != null) // && FileSigned.File.Data != null)
					{
						SaveFileToLocal (VehicleExitCardDocument, PDFPath);
					}
					return true;
				});
		}

		private void SaveFileToLocal(byte[] file, string path)
		{
			_fileStore.EnsureFolderExists(Attachments.ROOT);
			var filePath = _fileStore.PathCombine(Attachments.ROOT, path + ".pdf");
			_fileStore.WriteFile(filePath, file);
			OpenNovuSign = true;
		}
		#endregion


		/*public override IMvxCommand SyncCommand {
			get { return new MvxCommand (DoSync); }
		}

		private async void DoSync()
		{
			try
			{
				//_profileSvc = Mvx.Resolve<IProfileService> ();

				//RemoveDB();

				CheckDeviceConnectivity();

				bool success = await ShowLoadingForLongOperationAsync<bool>(async () => 
					{
						await _menuSvc.SyncAsync(Settings.LastUsername, Settings.LastPassword);
						LastSync = UpdateLastSync();

						return true;
					}
				);
			}
			catch (NavServiceAppOfflineException ex) {
				RaiseGeneralFailEvent (ex);
			}
			catch (WebException ex) {
				RaiseGeneralFailEvent (ex);
			}
			catch (Exception ex) {
				RaiseGeneralFailEvent(ex);
			}
		}*/
		#endregion

		protected override string UpdateLastSync ()
		{
			return _customer != null ? _customer.BusinessName + " - Ultima sincronizzazione: " + SyncLog.ReadTimeLog(ZumeroSync.NOVUSMOBILE_TITLE) : "Ultima sincronizzazione: " + SyncLog.ReadTimeLog(ZumeroSync.NAVSERVICE_TITLE);
		} 

		/*private string UpdateLastSync()
		{
			return _customer != null ? _customer.BusinessName + " - Ultima sincronizzazione: " + SyncLog.ReadTimeLog(ZumeroSync.NAVSERVICE_TITLE) : "Ultima sincronizzazione: " + SyncLog.ReadTimeLog(ZumeroSync.NAVSERVICE_TITLE);
		}*/
    }
}
