using HelloWorld.ViewModels;
using HelloWorld.Views;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;

namespace HelloWorld
{
    public class App : PrismApplication
    {
        protected override void OnInitialized()
        {
            NavigationService.NavigateAsync("ViewA", animated: false);
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<MyNavigationPage>();
            Container.RegisterTypeForNavigation<MyMasterDetail>();

            //override ViewModelLocator conventions and use a ViewModel based on its type
            ViewModelLocationProvider.Register<MainPage, SomeOtherViewModel>();
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog.AddModule(new ModuleInfo(typeof(ModuleA.ModuleAModule)));
            //ModuleCatalog.AddModule(new ModuleInfo("ModuleA", typeof(ModuleA.ModuleAModule), InitializationMode.OnDemand));
        }
    }
}
