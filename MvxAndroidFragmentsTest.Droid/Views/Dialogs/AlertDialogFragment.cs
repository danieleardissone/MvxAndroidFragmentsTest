#region Using Namespaces
using System;
using Android.OS;
using Android.Util;
using Android.App;
using Android.Content;
#endregion

namespace MolloOfficina.Droid.Views.Dialogs
{
	public interface IAlertDialogListener
	{
		void OnDialogClosed();
	}

	//[Register("NovusMobile.Droid.Views.Dialogs.AlertDialogFragment")]
	public class AlertDialogFragment : Android.Support.V4.App.DialogFragment
    {
		private const String ARG_MESSAGE = "message";
		private const String ARG_TITLE = "title";
		private const String ARG_ICON = "icon";
		private const String TAG = "AlertDialogFragment";

		private IAlertDialogListener _callback;

		// Default constructor must be kept public because the Fragment will be recreated by the Framework on Orientation Change
		public AlertDialogFragment()
		{
		}

		public static AlertDialogFragment NewInstance()
		{
			return NewInstance(String.Empty);
		}

		public static AlertDialogFragment NewInstance(String message)
		{
			return NewInstance(message, String.Empty);
		}

		public static AlertDialogFragment NewInstance(String message, String title)
		{
			return NewInstance(message, title, 0);
		}

		public static AlertDialogFragment NewInstance(String message, String title, int icon)
		{
			if (message == null)
				message = string.Empty;
			if (title == null)
				title = string.Empty;
			Bundle args = new Bundle();
			args.PutString(ARG_MESSAGE, message);
			args.PutString(ARG_TITLE, title);
			args.PutInt(ARG_ICON, icon);

			AlertDialogFragment alertDialogFragment = new AlertDialogFragment
			{
				Arguments = args, 
				Cancelable = false
			};
			return alertDialogFragment;
		}

		/*public override void OnAttach(Activity activity)
		{
			base.OnAttach(activity);
			_callback = activity as IAlertDialogListener;
		}*/

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            _callback = context as IAlertDialogListener;
        }

        public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Log.Debug(TAG, "OnCreate()");
			// RetainInstance = true;
		}

		public override void OnDestroyView()
		{
			// This workaround is needed during the orientation change... otherwise, the dialog will be dismissed
			// http://stackoverflow.com/questions/8235080/fragments-dialogfragment-and-screen-rotation
			if (Dialog != null && RetainInstance)
				Dialog.SetDismissMessage(null);
			base.OnDestroyView();
		}

		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			base.OnCreateDialog(savedInstanceState);
			Log.Debug(TAG, "OnCreateDialog()");

			Bundle bundle = Arguments;
			if (bundle == null)
				return null;
			String message = bundle.GetString(ARG_MESSAGE);
			String title = bundle.GetString(ARG_TITLE);
			int icon = bundle.GetInt(ARG_ICON);

			AlertDialog dlg =
				new AlertDialog.Builder(Activity).SetMessage(message).SetTitle(title).SetCancelable(false).Create();
			if (icon != 0)
				dlg.SetIcon(icon);
			dlg.SetButton("OK", (sender1, e1) =>
				{
					Log.Debug(TAG, "ClickButtonEvent()");
					Dismiss();
					if (_callback != null)
						_callback.OnDialogClosed();
				});
			return dlg;
		}
	}
}

