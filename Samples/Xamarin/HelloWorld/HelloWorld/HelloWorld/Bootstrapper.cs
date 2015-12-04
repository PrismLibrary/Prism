using HelloWorld.ViewModels;
using HelloWorld.Views;
using Prism.Navigation;
using Prism.Unity;

namespace HelloWorld
{
    //todo absolute uri
    //base application class
    public class Bootstrapper : UnityBootstrapper
    {
        protected override void OnInitialized()
        {
            NavigationService.Navigate("ViewA" + "ViewB" + "ViewC" + "MyNavPage/ViewD");


            var test = "ViewA/ViewB/ViewE";

            //MainPage if is the same, then go through the Nav stack.  Pop everything else.
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
