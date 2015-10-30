using HelloWorld.Views;
using Prism.Ninject;
using Xamarin.Forms;
using Ninject;

namespace HelloWorld
{
    public class Bootstrapper : NinjectBootstrapper
    {
        protected override Page CreateMainPage()
        {
            return Kernel.Get<MainPage>();
        }

        protected override void RegisterTypes()
        {
            Kernel.RegisterTypeForNavigation<ViewB>();
            Kernel.RegisterTypeForNavigation<ViewA>();
        }
    }
}
