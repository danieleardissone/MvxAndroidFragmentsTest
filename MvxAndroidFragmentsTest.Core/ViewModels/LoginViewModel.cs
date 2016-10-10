using FluentValidation;
using System;
using System.Net;
using Version.Plugin;
using System.IO;
using MvvmCross.Plugins.File;
using MvxAndroidFragmentsTest.Core.Resources;
using MvxAndroidFragmentsTest.Core.Helpers;
using MvxAndroidFragmentsTest.Core.Exceptions;
using MvvmCross.Core.ViewModels;
using MvxAndroidFragmentsTest.Core.Constants;
using Plugin.Permissions.Abstractions;
using MvxAndroidFragmentsTest.Core.Services;

namespace MvxAndroidFragmentsTest.Core.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{

        public LoginViewModel () : base()
		{
		
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

		private void DoLogin()
		{
                        //ShowViewModelAndClearBackStack<MenuViewModel>();  
		}
	}
}
