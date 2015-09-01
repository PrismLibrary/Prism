using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Unity.Windows;
using SplitViewNavigation.ViewModels;
using SplitViewNavigation.Views;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SplitViewNavigation
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : PrismUnityApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootFrame"></param>
        /// <returns></returns>
        protected override UIElement CreateShell(Frame rootFrame)
        {
            Debug.WriteLine("App.CreateShell()");
            ShellPageViewModel viewModel = Container.Resolve<ShellPageViewModel>();

            return new ShellPage(rootFrame, viewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            Debug.WriteLine("App.OnLaunchApplicationAsync()");
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 320));

            NavigationService.Navigate("Main", null);

            return Task.FromResult<object>(null);
        }
    }
}
