using HelloWorld.Views;
using Prism.Ioc;
using System.Windows;
using HelloWorld.Dialogs;

namespace HelloWorld
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        public static string ViewAName = "MyViewA";

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>(ViewAName);

            containerRegistry.RegisterDialog<NotificationDialog, NotificationDialogViewModel>();
            containerRegistry.RegisterDialog<ConfirmationDialog, ConfirmationDialogViewModel>();

            //register a custom window host
            containerRegistry.RegisterDialogWindow<CustomDialogWindow>();
            containerRegistry.RegisterDialogWindow<AnotherDialogWindow>(nameof(AnotherDialogWindow));
        }
    }
}
