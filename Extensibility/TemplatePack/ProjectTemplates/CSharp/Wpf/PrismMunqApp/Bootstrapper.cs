using Microsoft.Practices.Unity;
using Prism.Munq;
using $safeprojectname$.Views;
using System.Windows;

namespace $safeprojectname$
{
    class Bootstrapper : MunqBootstrapper
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
