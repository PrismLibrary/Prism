using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using HelloWorld.Services;
using HelloWorld.ViewModels;
using Prism.LightInject.Windows;

namespace HelloWorld
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : PrismLightInjectApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate("Main", null);
            return Task.FromResult<object>(null);
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.Register<IDataRepository, DataRepository>();
            Container.Register<MainPageViewModel>();
        }
    }
}