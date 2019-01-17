namespace Prism.Services.Dialogs
{
    public class DialogResult : IDialogResult
    {
        public IDialogParameters Parameters { get; private set; } = new DialogParameters();

        public bool? Result { get; private set; }

        public DialogResult() { }

        public DialogResult(bool? result)
        {
            Result = result;
        }

        public DialogResult(bool? result, IDialogParameters parameters)
        {
            Result = result;
            Parameters = parameters;
        }
    }
}
