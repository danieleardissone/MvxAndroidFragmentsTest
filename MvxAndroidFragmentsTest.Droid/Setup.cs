using Android.Content;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform;
using MvxAndroidFragmentsTest.Droid.Helpers;
using MvvmCross.Plugins.File;
using System.Collections.Generic;
using System.Reflection;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Droid.Views;
using MvvmCross.Droid.Shared.Presenter;
using MolloOfficina.Core.Services;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvxAndroidFragmentsTest.Core.Tasks;
using MvxAndroidFragmentsTest.Droid.Tasks;
using MvxAndroidFragmentsTest.Droid.Services;
using Acr.UserDialogs;
using MvvmCross.Platform.Droid.Platform;

namespace MvxAndroidFragmentsTest.Droid
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new Core.App();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override void InitializeFirstChance()
        {
            base.InitializeFirstChance();
            /*Mvx.RegisterType<IOpenExternalAppTask, OpenExternalAppTask>();
            Mvx.RegisterType<IProfileService, ProfileService>();
            Mvx.RegisterType<IMenuService, MenuService>();
            Mvx.RegisterType<INotificationService, NotificationService>();
            Mvx.RegisterType<IWorkOrderAttachmentService, WorkOrderAttachmentService>();
            Mvx.RegisterType<IWorkOrderHeaderService, WorkOrderHeaderService>();*/
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();
            Mvx.RegisterSingleton<MvxIoFileStoreBase>(new CustomMvxAndroidFileStore());

            UserDialogs.Init(() => Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity);
            //Mvx.RegisterSingleton<IMenuService>(new MenuService());
        }

        protected override IEnumerable<Assembly> AndroidViewAssemblies => new List<Assembly>(base.AndroidViewAssemblies)
        {
            typeof(Android.Support.Design.Widget.NavigationView).Assembly,
            typeof(Android.Support.Design.Widget.FloatingActionButton).Assembly,
            typeof(Android.Support.V7.Widget.Toolbar).Assembly,
            typeof(Android.Support.V4.Widget.DrawerLayout).Assembly,
            typeof(Android.Support.V4.View.ViewPager).Assembly,
            typeof(MvvmCross.Droid.Support.V7.RecyclerView.MvxRecyclerView).Assembly
        };

        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            MvxAppCompatSetupHelper.FillTargetFactories(registry);
            base.FillTargetFactories(registry);
        }

        /// <summary>
        /// This is very important to override. The default view presenter does not know how to show fragments!
        /// </summary>
        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            var mvxFragmentsPresenter = new MvxFragmentsPresenter(AndroidViewAssemblies);
            Mvx.RegisterSingleton<IMvxAndroidViewPresenter>(mvxFragmentsPresenter);
            return mvxFragmentsPresenter;
        }
    }
}
