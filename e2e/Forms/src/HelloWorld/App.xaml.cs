using System;
using System.Threading.Tasks;
using HelloWorld.ViewModels;
using HelloWorld.Views;
using Prism;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HelloWorld
{
    #region Test Navigation Calls

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
    #endregion
    public sealed partial class App
    {
        public App() 
            : this(null)
        {
        }

        public App(IPlatformInitializer initializer)
            : this(initializer, true)
        {
        }

        public App(IPlatformInitializer initializer, bool setFormsDependencyResolver)
            : base(initializer, setFormsDependencyResolver)
        {
        }

        protected override void OnInitialized()
        {
            InitializeComponent();
            this.ConfigureDefaultRegionBehaviors(behaviors =>
            {
                // Demo purposes only... you could add your own additional behaviors here...
                //behaviors.AddIfMissing(MyCustomBehavior.BehaviorName, typeof(MyCustomBehavior));
            });

            this.ConfigureRegionAdapterMappings((mappings, container) =>
            {
                // Demo purposes only... you could add aditional Adapter mappings here...
                // mappings.RegisterMapping(typeof(RefreshView), container.Resolve<MyCustomAdapter>());
            });

            NavigationService.NavigateAsync($"MyMasterDetail/MyTabbedPage").OnNavigationError(OnNavigationError);
        }

        private void OnNavigationError(Exception ex)
        {

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterRegionServices();
            containerRegistry.RegisterForNavigation<MainPage>();
            //containerRegistry.RegisterForNavigation<MainPage, SomeOtherViewModel>(); //override viewmodel convention
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MyNavigationPage>();
            containerRegistry.RegisterForNavigation<MyMasterDetail>();
            containerRegistry.RegisterForNavigation<ModulesPage, ModulesPageViewModel>();

            containerRegistry.RegisterForNavigation<ModulesPage, ModulesPageViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleA.ModuleAModule>();
            //moduleCatalog.AddModule(new ModuleInfo(typeof(ModuleA.ModuleAModule)));
            //moduleCatalog.AddModule(new ModuleInfo(typeof(ModuleA.ModuleAModule), "ModuleA", InitializationMode.OnDemand));
            moduleCatalog.AddModule<HelloDialog.HelloDialogModule>(InitializationMode.OnDemand);
            moduleCatalog.AddModule<HelloPageDialog.HelloPageDialogModule>(InitializationMode.OnDemand);
            moduleCatalog.AddModule<HelloRegions.RegionDemoModule>();
        }

        protected override void OnStart ()
        {
            // Handle when your app starts
        }

        protected override void OnSleep ()
        {
            // Support IApplicationLifecycleAware
            base.OnSleep();

            // Handle when your app sleeps
        }

        protected override void OnResume ()
        {
            // Support IApplicationLifecycleAware
            base.OnResume();

            // Handle when your app resumes
        }
    }
}
