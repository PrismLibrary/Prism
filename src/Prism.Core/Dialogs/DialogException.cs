using System;

namespace Prism.Dialogs;

/// <summary>
/// Represents errors that may occur within the <see cref="IDialogService"/>.
/// </summary>
public class DialogException : Exception
{
    /// <summary>
    /// The <see cref="DialogException"/> Message returned when an unexpected error occurred while displaying the dialog.
    /// </summary>
    public const string ShowDialog = "Error while displaying dialog";

    /// <summary>
    /// The <see cref="DialogException"/> Message returned when the CurrentPage must be a ContentPage
    /// </summary>
    /// <remarks>Xamarin.Forms &amp; Maui specific</remarks>
    public const string RequiresContentPage = "The current page must be a ContentPage";

    /// <summary>
    /// The <see cref="DialogException"/> Message returned when the Current View is not host a Dialog
    /// </summary>
    public const string HostPageIsNotDialogHost = "The current page is not currently hosting a Dialog";

    /// <summary>
    /// The <see cref="DialogException"/> Message returned when CanClose returns false
    /// </summary>
    public const string CanCloseIsFalse = "CanClose returned false";

    /// <summary>
    /// The <see cref="DialogException"/> Message returned when No ViewModel can be found
    /// </summary>
    public const string NoViewModel = "No ViewModel could be found";

    /// <summary>
    /// The <see cref="DialogException"/> Message returned when the ViewModel does not implement IDialogAware.
    /// </summary>
    public const string ImplementIDialogAware = "The ViewModel does not implement IDialogAware";

    /// <summary>
    /// The <see cref="DialogException"/> Message returned when Prism is unable to locate the backing field or setter for the <see cref="DialogCloseListener"/>.
    /// </summary>
    public const string UnableToSetTheDialogCloseListener = "Unable to locate the backing field or setter for IDialogAware.RequestClose";

    /// <summary>
    /// Initializes a new <see cref="DialogException"/> with a given message
    /// </summary>
    /// <param name="message"></param>
    public DialogException(string message) : base(message)
    {
    }
}
