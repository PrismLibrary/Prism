using HelloWorld.ViewModels;
using HelloWorld.Views;
using Prism;
using Prism.Common;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Unity;
using System.Linq;
using Xamarin.Forms;
using Prism.Ioc;

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

            NavigationService.NavigateAsync($"NavigationPage/ViewA/ViewB/ViewC");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage>();
            //containerRegistry.RegisterForNavigation<MainPage, SomeOtherViewModel>(); //override viewmodel convention
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MyNavigationPage>();
            containerRegistry.RegisterForNavigation<MyMasterDetail>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleA.ModuleAModule>();
            //moduleCatalog.AddModule(new ModuleInfo(typeof(ModuleA.ModuleAModule)));
            //moduleCatalog.AddModule(new ModuleInfo(typeof(ModuleA.ModuleAModule), "ModuleA", InitializationMode.OnDemand));
        }
    }
}
