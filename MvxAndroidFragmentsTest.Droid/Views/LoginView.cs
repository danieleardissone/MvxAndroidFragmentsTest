using Android.App;
using Android.OS;
using Android.Content.PM;
using System.IO;
using Android.Runtime;
using Android.Views;
using MolloOfficina.Droid.Views;
using MolloOfficina.Core.ViewModels;
using MolloOfficina.Core.Constants;

namespace MolloOfficina.Droid.Views
{
	[Activity(Label = "@string/login_title", LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateHidden, Name = "molloofficina.droid.views.LoginView")]
	public class LoginView : BaseActivityView<LoginViewModel> //MvxActivity
    {
		/*protected new LoginViewModel ViewModel
		{
			get { return base.ViewModel as LoginViewModel; }
		}*/

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

			//CopyDataBase ();


			//ActionBar.Hide ();
			//RequestWindowFeature (Android.Views.WindowFeatures.NoTitle);
			//Window.SetFlags (Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);

            SetContentView(Resource.Layout.LoginView);
			this.Window.SetSoftInputMode (SoftInput.StateHidden);
			/*txtUsername = FindViewById<EditText> (Resource.Id.txtUsername);
			txtPassword = FindViewById<EditText> (Resource.Id.txtPassword);
			btnLogin = FindViewById<Button> (Resource.Id.btnLogin);

			#region Custom Fonts

			Typeface customFont = Typeface.CreateFromAsset (Assets, GetString (Resource.String.font_path_myriadpro_regular));
			txtUsername.SetTypeface (customFont, TypefaceStyle.Normal);
			txtPassword.SetTypeface (customFont, TypefaceStyle.Normal);
			btnLogin.SetTypeface (customFont, TypefaceStyle.Normal);
			#endregion*/

			/*btnLogin.Click += (object sender, System.EventArgs e) => {
				ResetTextFieldsRightView();
				((LoginViewModel)this.ViewModel).LoginCommand.Execute ();
			};*/
        }
			
		/*protected override void AttachBaseContext(Context context)
		{
			base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
		}*/

		private void ResetTextFieldsRightView()
		{
			
			//txtUsername.RightViewMode = UITextFieldViewMode.Never;
			//txtPassword.RightViewMode = UITextFieldViewMode.Never;
		}

		//The Android's default system path of your application database.
		//private string DB_PATH = "/data/data/YOUR_PACKAGE/databases/";
		//private string DB_NAME = "NovusNavService_001.db";

		private void CopyDataBase() 
		{
			//string dbName = "db.sqlite";
			//string dbPath = Path.Combine (Android.OS.Environment.ExternalStorageDirectory.ToString (), dbName);
			string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal).ToString(), ZumeroSync.DB_NAME); //Android.OS.Environment.ExternalStorageDirectory.Path + "/NavService/" + SQLite.DB_NAME;
			// Check if your DB has already been extracted.
			if (!File.Exists(dbPath))
			{
				using (BinaryReader br = new BinaryReader(Assets.Open(ZumeroSync.DB_NAME)))
				{
					using (BinaryWriter bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
					{
						byte[] buffer = new byte[2048];
						int len = 0;
						while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
						{
							bw.Write (buffer, 0, len);
						}
					}
				}
			}
		}
    }
}
