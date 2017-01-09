using Microsoft.Practices.Mef;
using Prism.Mef;
using $safeprojectname$.Views;
using System.Windows;

namespace $safeprojectname$
{
    class Bootstrapper : MefBootstrapper
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
