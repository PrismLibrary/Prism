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
            : this(initializer, false)
        {
        }

        public App(IPlatformInitializer initializer, bool setFormsDependencyResolver)
            : base(initializer, setFormsDependencyResolver)
        {
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            // DO NOT COMMIT CHANGES TO THIS!!!!
            NavigationService.NavigateAsync($"MyFlyout/MyTabbedPage").OnNavigationError(OnNavigationError);
        }

        private void OnNavigationError(Exception ex)
        {
            if(ex.InnerException is ContainerResolutionException cre)
            {
                var errors = cre.GetErrors();
                foreach(var error in errors)
                {
                    Console.WriteLine();
                    Console.WriteLine(error.Key.FullName);
                    Console.WriteLine(error.Value.ToString());
                    Console.WriteLine();
                }
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage>();
            //containerRegistry.RegisterForNavigation<MainPage, SomeOtherViewModel>(); //override viewmodel convention
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MyNavigationPage>();
            containerRegistry.RegisterForNavigation<MyFlyout>();
            containerRegistry.RegisterForNavigation<ModulesPage, ModulesPageViewModel>();

            containerRegistry.RegisterForNavigation<ModulesPage, ModulesPageViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleA.ModuleAModule>();
            moduleCatalog.AddModule<HelloDialog.HelloDialogModule>(InitializationMode.OnDemand);
            moduleCatalog.AddModule(new ModuleInfo(typeof(HelloPageDialog.HelloPageDialogModule), "HelloPageDialogModule", InitializationMode.OnDemand));
            moduleCatalog.AddModule(new ModuleInfo(typeof(HelloRegions.RegionDemoModule)));
        }

        protected override void InitializeModules()
        {
            var manager = Container.Resolve<IModuleManager>();
            manager.LoadModuleCompleted += OnLoadModuleCompleted;
            base.InitializeModules();
        }

        private void OnLoadModuleCompleted(object sender, LoadModuleCompletedEventArgs e)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                return;

            if(e.Error != null || e.ModuleInfo.State != ModuleState.Initialized)
            {
                System.Diagnostics.Debugger.Break();
            }
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
