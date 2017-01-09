using Ninject;
using Prism.Ninject;
using $safeprojectname$.Views;
using System.Windows;

namespace $safeprojectname$
{
    class Bootstrapper : NinjectBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Kernel.TryGet<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
