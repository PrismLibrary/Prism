using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism.Ioc;

#nullable enable
namespace Prism.Dialogs;

public partial class DialogService
{
    // ContentDialog host selection and presentation are identical for Uno and native
    // WinUI. The caller retains ownership of window selection and close lifecycle.
    private static IDialogWindow CreateDialogWindow(IContainerProvider provider, string? name) =>
        string.IsNullOrWhiteSpace(name) ? provider.Resolve<IDialogWindow>() : provider.Resolve<IDialogWindow>(name);

    private static ContentDialogPlacement GetDialogPlacement(IDialogParameters parameters) =>
        parameters.ContainsKey(KnownDialogParameters.DialogPlacement)
            ? parameters[KnownDialogParameters.DialogPlacement] is ContentDialogPlacement placement
                ? placement
                : Enum.Parse<ContentDialogPlacement>(parameters[KnownDialogParameters.DialogPlacement]?.ToString() ?? string.Empty)
            : ContentDialogPlacement.Popup;

    private static void ConfigureDialogWindowProperties(IDialogWindow window, FrameworkElement dialogContent, IDialogAware viewModel, bool resetResult = false)
    {
        var windowStyle = Dialog.GetWindowStyle(dialogContent);
        if (windowStyle != null)
            window.Style = windowStyle;

        if (resetResult)
            window.Result = null;

        window.Content = dialogContent;
        window.DataContext = viewModel;
    }
}
