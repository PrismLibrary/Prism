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

#if !HAS_WINUI
    /// <summary>
    /// Flag to show the Dialog Modally or Non-Modally
    /// </summary>
    public const string ShowNonModal = "nonModal";
#endif
}
