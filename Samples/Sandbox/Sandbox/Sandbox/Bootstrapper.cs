using Prism.Unity;
using Sandbox.ViewModels;
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

        protected override void RegisterTypes()
        {
            //default convention - NavigationService.Navigate("ViewA");
            Container.RegisterTypeForNavigation<ViewA>();

            //provide custom string as a unique name - NavigationService.Navigate("B");
            Container.RegisterTypeForNavigation<ViewB>("B");

            //use a ViewModel class to act as the unique name - NavigationService.Navigate<ViewCViewModel>();
            Container.RegisterTypeForNavigation<ViewC, ViewCViewModel>();
        }
    }
}
