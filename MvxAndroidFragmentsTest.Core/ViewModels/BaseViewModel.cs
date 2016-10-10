using System;
using System.Linq;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Acr.UserDialogs;
using System.Collections;
using System.Threading.Tasks;
using Acr.DeviceInfo;
using System.Net;
using System.Reactive.Linq;
using Plugin.Connectivity;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using MvvmCross.Platform;
using MvxAndroidFragmentsTest.Core.Entities;
using MvxAndroidFragmentsTest.Core.Resources;
using MvxAndroidFragmentsTest.Core.Exceptions;

namespace MvxAndroidFragmentsTest.Core.ViewModels
{
	public class BaseViewModel : MvxViewModel, INotifyDataErrorInfo
	{
		public BaseViewModel ()
		{

		}

		protected IMvxMessenger MvxMessenger {
			get {
				return Mvx.Resolve<IMvxMessenger>();
			}
		}

		protected MvxSubscriptionToken Subscribe<TMessage> (Action<TMessage> action)
			where TMessage : MvxMessage
		{
			return MvxMessenger.Subscribe<TMessage>(action, MvxReference.Weak);
		}

		protected void Unsubscribe<TMessage> (MvxSubscriptionToken id)
			where TMessage : MvxMessage
		{
			MvxMessenger.Unsubscribe<TMessage>(id);
		}

        public virtual IMvxCommand RefreshViewStateCommand { get { return null; } }

		#region Parameteres
		private bool _isBusy;
		public bool IsBusy {
			get {
				return _isBusy;
			}
			set {
				_isBusy = value;
				RaisePropertyChanged (() => IsBusy);
			}
		}
		#endregion

		#region ViewModel lifecycle
		protected override void InitFromBundle (IMvxBundle parameters)
		{
			base.InitFromBundle (parameters);
		}

		protected override void SaveStateToBundle (IMvxBundle bundle)
		{
			base.SaveStateToBundle (bundle);
		}

		protected override void ReloadFromBundle (IMvxBundle state)
		{
			base.ReloadFromBundle (state);
		}
		#endregion 

		#region Events
		public event EventHandler<EventArgs<Exception>> GeneralFailure;
		protected void RaiseGeneralFailEvent(Exception ex)
		{
			OnGeneralFailure (ex);
		}

		protected virtual void OnGeneralFailure (Exception ex)
		{
			var generaleFailure = GeneralFailure;
			if (generaleFailure == null)
				return;

			InvokeOnMainThread (() => {
				generaleFailure(this, new EventArgs<Exception>(ex));
			});
		}
		#endregion

		#region Validation - INotifyDataErrorInfo
		private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
		private object _validateLock = new object ();
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
		protected IValidator Validator { get; set; }

		public Dictionary<string, List<string>> Errors
		{
			get { return _errors; }
		}

		protected void RaiseErrorsChangedEvent([CallerMemberName] string propertyName = null)
		{
			OnErrorsChanged(propertyName);
		}

		protected void OnErrorsChanged([CallerMemberName] string propertyName = null)
		{
			var errorsChanged = ErrorsChanged;
			if (errorsChanged == null)
				return;
			InvokeOnMainThread(() =>
				errorsChanged(this, new DataErrorsChangedEventArgs(propertyName)));
		}

		public bool HasErrors
		{
			get { return _errors.Any (propErrors => propErrors.Value != null && propErrors.Value.Count > 0); }
		}

		public IEnumerable GetErrors([CallerMemberName] string propertyName = null)
		{
			if (string.IsNullOrEmpty (propertyName)) {
				return _errors.SelectMany (err => err.Value.ToList ());
			}

			if (!_errors.ContainsKey(propertyName) || _errors[propertyName] == null) // || _errors[propertyName].Count > 0)
				return new List<string>();
			
			return _errors[propertyName];
		}

		public bool Validate()
		{
			lock (_validateLock) 
			{
				if (Validator == null)
					return true;

				ValidationResult results = Validator.Validate (this);

				//Clear all previous errors
				_errors.Clear();
				var propNames = _errors.Keys.ToList();
				//propNames.ForEach(pn => OnErrorsChanged(pn));

				HandleValidationFailures(results.Errors);
				RaisePropertyChanged(() => Errors);
				return (results.Errors.Count == 0);
			}
		}

		private void HandleValidationFailures(IList<ValidationFailure> aFailureList)
		{
			if (aFailureList == null || aFailureList.Count() == 0)
				return;
			//Group validation results by property names
			var resultsByPropNames = from res in aFailureList
				group res by res.PropertyName into g
				select g;
			//add errors to dictionary and inform  binding engine about errors
			foreach (var prop in resultsByPropNames)
			{
				var messages = prop.Select(r => r.ErrorMessage).ToList();
				if (_errors.ContainsKey(prop.Key))
				{
					_errors[prop.Key].AddRange(messages);
				}
				else
				{
					//ErrorsCollection err_coll = new ErrorsCollection(messages);
					_errors.Add(prop.Key, messages);
				}
				OnErrorsChanged(prop.Key);
			}
		}
		#endregion

		#region Navigation and Dialogs
		protected void ShowViewModelAndClearBackStack<T>(IMvxBundle paramBundle = null) where T : MvxViewModel
		{
			ShowViewModel<T>(paramBundle);
		}

		protected void ShowMessageBox(string title, string message) //, Action onOKAction)
		{
			IUserDialogs dlg = Mvx.Resolve<IUserDialogs> ();
			if (dlg == null)
				return;

			InvokeOnMainThread (() => {
				AlertConfig cfg = new AlertConfig ();
				cfg.SetTitle(title);
				cfg.SetMessage(message);
				//cfg.OnAction = onOKAction;
				dlg.Alert (cfg);
			});
		}

		protected void ShowConfirmMessageBox(string title, string message, Action<bool> onConfirmAction)
		{
			IUserDialogs dlg = Mvx.Resolve<IUserDialogs> ();
			if (dlg == null)
				return;

			InvokeOnMainThread (() => {
				ConfirmConfig cfg = new ConfirmConfig ();
				cfg.SetTitle(title);
				cfg.SetMessage(message);
				cfg.UseYesNo();
				cfg.SetOkText("Si");
				cfg.SetAction(onConfirmAction);
				dlg.Confirm (cfg);
			});
		}

		private void ShowProgressDialog()
		{
			IUserDialogs dlg = Mvx.Resolve<IUserDialogs> ();
			if (dlg == null)
				return;
			InvokeOnMainThread (() => dlg.ShowLoading (string.Empty, null));
		}

		private void ShowProgressDialogWithMessage(string message)
		{
			IUserDialogs dlg = Mvx.Resolve<IUserDialogs> ();
			if (dlg == null)
				return;

			InvokeOnMainThread (() => dlg.ShowLoading (message, null));
		}

		private void HideProgressDialog()
		{
			IUserDialogs dlg = Mvx.Resolve<IUserDialogs> ();
			if (dlg == null)
				return;
			InvokeOnMainThread (() => dlg.HideLoading ());
		}
		#endregion
    }
}

