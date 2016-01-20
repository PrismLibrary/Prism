using HelloWorld.Views;
using Prism.Unity;

namespace HelloWorld
{
    public class App : PrismApplication
    {
        protected override void OnInitialized()
        {
            NavigationService.Navigate("MyMasterDetail/MyNavigationPage/ViewA/ViewB", animated: false);
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<MyNavigationPage>();
            Container.RegisterTypeForNavigation<MyTabbedPage>();
            Container.RegisterTypeForNavigation<MyMasterDetail>();
            Container.RegisterTypeForNavigation<ViewA>();
            Container.RegisterTypeForNavigation<ViewB>();
            Container.RegisterTypeForNavigation<ViewC>();
        }
    }
}
