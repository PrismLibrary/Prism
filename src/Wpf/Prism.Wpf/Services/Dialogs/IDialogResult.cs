namespace Prism.Services.Dialogs
{
    /// <summary>
    /// Contains <see cref="IDialogParameters"/> from the dialog
    /// and the <see cref="ButtonResult"/> of the dialog.
    /// </summary>
    public interface IDialogResult
    {
        /// <summary>
        /// The parameters from the dialog.
        /// </summary>
        IDialogParameters Parameters { get; }

        /// <summary>
        /// The result of the dialog.
        /// </summary>
        ButtonResult Result { get; }
    }
}
