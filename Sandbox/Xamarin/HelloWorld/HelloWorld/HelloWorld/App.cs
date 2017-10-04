using HelloWorld.ViewModels;
using HelloWorld.Views;
using Prism.Common;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Unity;
using System.Linq;
using Xamarin.Forms;

namespace HelloWorld
{
    public class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            //NavigationService.NavigateAsync("NavigationPage/MyTabbedPage", animated: false); //works
            //NavigationService.NavigateAsync("NavigationPage/MyTabbedPage/ViewC", animated: false); //works
            //NavigationService.NavigateAsync("NavigationPage/MyTabbedPage/ViewC/ViewA", animated: false); //works
            //NavigationService.NavigateAsync("NavigationPage/ViewA/MyTabbedPage", animated: false); //works
            //NavigationService.NavigateAsync("NavigationPage/ViewA/MyTabbedPage/ViewC", animated: false); //works
            //NavigationService.NavigateAsync("NavigationPage/ViewA/MyTabbedPage/ViewC/ViewA/ViewB", animated: false); //works

            NavigationService.NavigateAsync("MyMasterDetail/NavigationPage/MyTabbedPage/ViewC", animated: false); //
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainPage, SomeOtherViewModel>(); //override viewmodel convention
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MyNavigationPage>();
            Container.RegisterTypeForNavigation<MyMasterDetail>();
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog.AddModule(new ModuleInfo(typeof(ModuleA.ModuleAModule)));
            //ModuleCatalog.AddModule(new ModuleInfo("ModuleA", typeof(ModuleA.ModuleAModule), InitializationMode.OnDemand));
        }
    }
}
