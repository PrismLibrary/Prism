using Prism.Unity;
using Sandbox.Views;
using Xamarin.Forms;

namespace Sandbox
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override Page CreateMainPage()
        {
            return new MainPage();
        }

        protected override void InitializeMainPage()
        {
            //use if CreateMainPage returns a NavigationPage   
            //when navigating within a NavigationPage, set useModalNavigation = false
            //NavigationService.Navigate("ViewA");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<ViewB>("ViewA");
            Container.RegisterTypeForNavigation<ViewB>("ViewB");
            Container.RegisterTypeForNavigation<ViewC>("ViewC");
        }
    }
}
