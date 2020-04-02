using Prism.Regions;

namespace Prism.Services.Dialogs
{
    public class DialogParameters : NavigationParameters, IDialogParameters
    {
        public DialogParameters() : base() { }

        public DialogParameters(string query) : base(query) { }
    }
}
