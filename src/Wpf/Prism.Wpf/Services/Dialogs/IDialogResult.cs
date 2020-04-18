namespace Prism.Services.Dialogs
{
    /// <summary>
    /// Contains <see cref="IDialogParameters"/> passed to the dialog
    /// and the <see cref="ButtonResult"/> of the dialog.
    /// </summary>
    public interface IDialogResult
    {
        /// <summary>
        /// The parameters passed to the dialog.
        /// </summary>
        IDialogParameters Parameters { get; }

        /// <summary>
        /// The result of the dialog.
        /// </summary>
        ButtonResult Result { get; }        
    }
}
