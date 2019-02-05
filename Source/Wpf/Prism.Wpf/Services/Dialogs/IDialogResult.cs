namespace Prism.Services.Dialogs
{
    public interface IDialogResult
    {
        /// <summary>
        /// The parameters from the dialog
        /// </summary>
        IDialogParameters Parameters { get; }

        /// <summary>
        /// The result of the dialog.
        /// </summary>
        bool? Result { get; }        
    }
}
