using System;
using Android.Content;
using Android.App;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Plugins.Messenger;
using MvvmCross.Platform;

namespace MvxAndroidFragmentsTest.Droid.Views.Dialogs
{
	public class BaseDialogFragment : MvxDialogFragment, IDialogInterface
	{
		protected Context Context;
		protected IDialogInterfaceOnDismissListener DismissListener = null;
		protected IDialogInterfaceOnClickListener ClickListener = null;

		public BaseDialogFragment (Context context) : base()
		{
			Context = context;
		}

		public void SetOnDismissListener(IDialogInterfaceOnDismissListener listener)
		{
			DismissListener = listener;
		}

		public void SetOnClickListener(IDialogInterfaceOnClickListener listener)
		{
			ClickListener = listener;
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

		public void Cancel()
		{
			Dialog.Cancel ();
			Dismiss ();
		}
	}
}
