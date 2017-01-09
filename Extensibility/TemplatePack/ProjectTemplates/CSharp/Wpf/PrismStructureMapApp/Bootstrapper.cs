using Microsoft.Practices.StructureMap;
using Prism.StructureMap;
using $safeprojectname$.Views;
using System.Windows;

namespace $safeprojectname$
{
    class Bootstrapper : StructureMapBootstrapper
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
