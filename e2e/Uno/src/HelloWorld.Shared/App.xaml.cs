using Windows.UI.Xaml;
using HelloWorld.Views;
using Prism.DryIoc;
using Prism.Ioc;

namespace HelloWorld
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : PrismApplication
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override UIElement CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
