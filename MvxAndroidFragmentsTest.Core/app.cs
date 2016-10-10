using Acr.UserDialogs;
using MvxAndroidFragmentsTest.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;

namespace MvxAndroidFragmentsTest.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            Mvx.RegisterSingleton<IUserDialogs>(() => UserDialogs.Instance);

            RegisterAppStart<LoginViewModel>();
        }
    }
}
