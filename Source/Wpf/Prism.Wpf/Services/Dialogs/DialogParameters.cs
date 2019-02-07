using Prism.Regions;

namespace Prism.Services.Dialogs
{
    //TODO: should we reuse the NavigationParameters? I'm not sure I want to add the regions namespace requirement for using dialogs
    public class DialogParameters : NavigationParameters, IDialogParameters
    {
        public DialogParameters() : base() { }

        public DialogParameters(string query) : base(query) { }
    }
}
