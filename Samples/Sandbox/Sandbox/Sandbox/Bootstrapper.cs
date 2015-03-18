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

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<ViewA>();
            Container.RegisterTypeForNavigation<ViewB>();
            Container.RegisterTypeForNavigation<ViewC>("ViewCKey");
        }
    }
}
