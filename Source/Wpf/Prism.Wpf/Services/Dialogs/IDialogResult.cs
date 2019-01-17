namespace Prism.Services.Dialogs
{
    public interface IDialogResult
    {
        IDialogParameters Parameters { get; }

        bool? Result { get; }        
    }
}
