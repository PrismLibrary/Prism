namespace Prism.Services.Dialogs
{
    public class DialogResult : IDialogResult
    {
        public IDialogParameters Parameters { get; private set; } = new DialogParameters();

        public ButtonResult Result { get; private set; } = ButtonResult.None;

        public DialogResult() { }

        public DialogResult(ButtonResult result)
        {
            Result = result;
        }

        public DialogResult(ButtonResult result, IDialogParameters parameters)
        {
            Result = result;
            Parameters = parameters;
        }
    }
}
