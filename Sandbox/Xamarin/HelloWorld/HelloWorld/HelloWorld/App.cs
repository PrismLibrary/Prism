using HelloWorld.Views;
using Prism.Unity;

namespace HelloWorld
{
    public class App : PrismApplication
    {
        protected override void OnInitialized()
        {
            NavigationService.Navigate("MyMasterDetail/MyNavigationPage/ViewA");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<MyNavigationPage>();
            Container.RegisterTypeForNavigation<MyMasterDetail>();
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog.AddModule(typeof(ModuleA.ModuleAModule));
        }
    }
}
