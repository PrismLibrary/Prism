using Microsoft.Practices.Unity;
using Prism.Unity;
using $safeprojectname$.Views;
using System.Windows;

namespace $safeprojectname$
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
