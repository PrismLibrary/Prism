using Microsoft.Practices.Unity;
using Prism.Unity;
using UsingPopupWindowAction.Views;
using System.Windows;

namespace UsingPopupWindowAction
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
