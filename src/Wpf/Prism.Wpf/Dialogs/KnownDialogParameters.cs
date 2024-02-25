namespace Prism.Dialogs;

/// <summary>
/// Provides Dialog Parameter Keys for well known paramotors used by the <see cref="IDialogService"/>
/// </summary>
public static class KnownDialogParameters
{
    /// <summary>
    /// The name of the window
    /// </summary>
    public const string WindowName = "windowName";

#if UNO_WINUI
    /// <summary>
    /// The <see cref="Microsoft.UI.Xaml.Controls.ContentDialogPlacement"/> to use when showing the dialog
    /// </summary>
    public const string DialogPlacement = "dialogPlacement";
#else
    /// <summary>
    /// Flag to show the Dialog Modally or Non-Modally
    /// </summary>
    public const string ShowNonModal = "nonModal";
#endif
}
