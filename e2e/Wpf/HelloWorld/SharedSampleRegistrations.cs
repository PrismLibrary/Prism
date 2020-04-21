using HelloWorld.Dialogs;
using Prism.Ioc;

namespace HelloWorld
{
    static class SharedSampleRegistrations
    {
        public static void RegisterSharedSamples(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<NotificationDialog, NotificationDialogViewModel>();
            containerRegistry.RegisterDialog<ConfirmationDialog, ConfirmationDialogViewModel>();

            //register a custom window host
            containerRegistry.RegisterDialogWindow<CustomDialogWindow>();
            containerRegistry.RegisterDialogWindow<AnotherDialogWindow>(nameof(AnotherDialogWindow));
        }
    }
}
