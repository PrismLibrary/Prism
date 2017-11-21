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
            //NavigationService.NavigateAsync("NavigationPage/MyTabbedPage"); //works
            //NavigationService.NavigateAsync("NavigationPage/MyTabbedPage/ViewC"); //works
            //NavigationService.NavigateAsync("NavigationPage/MyTabbedPage/ViewC/ViewA"); //works
            //NavigationService.NavigateAsync("NavigationPage/ViewA/MyTabbedPage"); //works
            //NavigationService.NavigateAsync("NavigationPage/ViewA/MyTabbedPage/ViewC"); //works
            //NavigationService.NavigateAsync("NavigationPage/ViewA/MyTabbedPage/ViewC/ViewA/ViewB"); //works
            //NavigationService.NavigateAsync("MyMasterDetail/NavigationPage/MyTabbedPage/ViewC"); //works

            //NavigationService.NavigateAsync($"MyTabbedPage?{KnownNavigationParameters.SelectedTab}=ViewC/ViewA"); //works --
            //NavigationService.NavigateAsync($"NavigationPage/MyTabbedPage?{KnownNavigationParameters.SelectedTab}=ViewC"); //works
            //NavigationService.NavigateAsync($"NavigationPage/MyTabbedPage?{KnownNavigationParameters.SelectedTab}=ViewC/ViewC"); //works
            //NavigationService.NavigateAsync($"NavigationPage/MyTabbedPage?{KnownNavigationParameters.SelectedTab}=ViewC/ViewC/ViewA"); //works
            //NavigationService.NavigateAsync($"NavigationPage/ViewA/MyTabbedPage?{KnownNavigationParameters.SelectedTab}=ViewC"); //works
            //NavigationService.NavigateAsync($"NavigationPage/ViewA/MyTabbedPage?{KnownNavigationParameters.SelectedTab}=ViewC/ViewC"); //works
            //NavigationService.NavigateAsync($"NavigationPage/ViewA/MyTabbedPage?{KnownNavigationParameters.SelectedTab}=ViewC/ViewC/ViewA/ViewB"); //works
            //NavigationService.NavigateAsync($"MyMasterDetail/NavigationPage/MyTabbedPage?{KnownNavigationParameters.SelectedTab}=ViewC/ViewC"); //works

            //NavigationService.NavigateAsync($"NavigationPage/ViewA/MyTabbedPage?{KnownNavigationParameters.SelectedTab}=ViewC/ViewC");
            //NavigationService.NavigateAsync($"NavigationPage/ViewA/MyTabbedPage/ViewA/ViewB/ViewC");
            //NavigationService.NavigateAsync($"NavigationPage/ViewA/ViewB/NavigationPage?{KnownNavigationParameters.UseModalNavigation}=true/ViewB/ViewC");
            //NavigationService.NavigateAsync($"NavigationPage/ViewA/ViewB/ViewC?{KnownNavigationParameters.UseModalNavigation}=true");
            //NavigationService.NavigateAsync($"MyMasterDetail/NavigationPage/MyTabbedPage/ViewA/ViewC?{KnownNavigationParameters.UseModalNavigation}=true");
            //NavigationService.NavigateAsync($"ViewA/ViewB/MyMasterDetail/NavigationPage/ViewA/ViewC");
            //NavigationService.NavigateAsync($"ViewA/ViewB/MyMasterDetail/ViewA/ViewC");
            //NavigationService.NavigateAsync($"ViewA/ViewB/MyMasterDetail/NavigationPage/ViewA/ViewB?{KnownNavigationParameters.UseModalNavigation}=true/ViewA/ViewC");
            //NavigationService.NavigateAsync($"MyMasterDetail/NavigationPage/MyTabbedPage?{KnownNavigationParameters.SelectedTab}=ViewC");            

            NavigationService.NavigateAsync($"ViewA/ViewB/MyMasterDetail/NavigationPage/ViewA/ViewB?{KnownNavigationParameters.UseModalNavigation}=true/ViewA/ViewC");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainPage>(); 
            //Container.RegisterTypeForNavigation<MainPage, SomeOtherViewModel>(); //override viewmodel convention
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MyNavigationPage>();
            Container.RegisterTypeForNavigation<MyMasterDetail>();
        }

        protected override void ConfigureModuleCatalog()
        {            
            ModuleCatalog.AddModule<ModuleA.ModuleAModule>();
            //ModuleCatalog.AddModule(new ModuleInfo(typeof(ModuleA.ModuleAModule)));
            //ModuleCatalog.AddModule(new ModuleInfo("ModuleA", typeof(ModuleA.ModuleAModule), InitializationMode.OnDemand));
        }
    }
}
