using HelloWorld.Views;
using Prism.Ninject;
using Prism.Ioc;
using System.Windows;
using HelloWorld.Dialogs;

namespace HelloWorld
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<NotificationDialog, NotificationDialogViewModel>();
        }
    }
}
