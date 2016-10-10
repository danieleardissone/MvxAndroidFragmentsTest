using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V4;
using MolloOfficina.Core.ViewModels;
using MolloOfficina.Core.Enums;
using System.Net;
using MolloOfficina.Core.Exceptions;
using MolloOfficina.Core.Entities;
using MolloOfficina.Droid.Views.Dialogs;

namespace MolloOfficina.Droid.Views.Fragments
{
    public abstract class BaseFragment : MvxFragment
    {
        private const string TAG = "BaseFragmentView";
        private const string NOTIFICATION_FRAGMENT_TAG = "NotificationFragment";

        protected BaseFragment()
        {
            this.RetainInstance = true;
        }

        protected abstract int FragmentId { get; }

        protected new BaseViewModel ViewModel
        {
            get { return base.ViewModel as BaseViewModel; }
            //set { base.ViewModel = value; }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnResume()
        {
            base.OnResume();

            ViewModel.GeneralFailure += HandleGeneralFailure;
        }

        public override void OnPause()
        {
            base.OnPause();

            ViewModel.GeneralFailure -= HandleGeneralFailure;
        }

        protected virtual void HandleGeneralFailure(object sender, EventArgs<Exception> e)
        {
            if (e.Value is MolloOfficinaOfflineException || e.Value is WebException)
            {
                //new OfflineDialogFragment(Activity).Show(ChildFragmentManager, "OfflineDialogFragment");
                ShowAlertDialogFragment(e.Value.Message, MessageTypeEnum.Offline);
                return;
            }
            ShowAlertDialogFragment(e.Value.Message, MessageTypeEnum.Error);
        }

        protected void ShowAlertDialogFragment(string aMessage, MessageTypeEnum aType)
        {
            switch (aType)
            {
                case MessageTypeEnum.Information:
                    ShowAlertDialogFragment(aMessage, Resources.GetString(Resource.String.common_information), Resource.Drawable.ic_dialog_information_shadow);
                    break;
                case MessageTypeEnum.Warning:
                    ShowAlertDialogFragment(aMessage, Resources.GetString(Resource.String.common_warning), Resource.Drawable.ic_dialog_warning_shadow);
                    break;
                case MessageTypeEnum.Error:
                    ShowAlertDialogFragment(aMessage, Resources.GetString(Resource.String.common_error), Resource.Drawable.ic_dialog_error_shadow);
                    break;
                case MessageTypeEnum.Offline:
                    ShowAlertDialogFragment(aMessage, Resources.GetString(Resource.String.common_offline), Resource.Drawable.ic_dialog_error_shadow);
                    break;
                default:
                    break;
            }
        }

        protected void ShowAlertDialogFragment(string aMessage, string aTitle, int aIcon)
        {
            // TODO:

            //if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
            //{	// IsDestroyed has been introduced since API Level 17
            if (ChildFragmentManager.IsDestroyed)
            {
                Log.Error(TAG, "Activity has been already destroyed");
                return;
            }
            //} /**/
            AlertDialogFragment frag = AlertDialogFragment.NewInstance(aMessage, aTitle, aIcon);
            ChildFragmentManager.BeginTransaction().Add(frag, NOTIFICATION_FRAGMENT_TAG + frag.GetHashCode()).Commit();
        }

        public virtual void PerformCustomAction()
        {
        }
    }

    public abstract class BaseFragment<TViewModel> : BaseFragment where TViewModel : class, IMvxViewModel
    {
        public new TViewModel ViewModel
        {
            get { return base.ViewModel as TViewModel; }
            //set { base.ViewModel = value; }
        }
    }
}