using HelloWorld.ViewModels;
using HelloWorld.Views;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;

namespace HelloWorld
{
    public class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            NavigationService.NavigateAsync("MyMasterDetail/MyNavigationPage/MainPage", animated: false);
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainPage, SomeOtherViewModel>(); //override viewmodel convention
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
