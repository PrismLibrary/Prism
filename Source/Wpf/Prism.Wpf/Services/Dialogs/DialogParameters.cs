using Prism.Regions;

namespace Prism.Services.Dialogs
{
    //TODO: should we reuse the NavigatrionParameters? I'm not sure I want to add the regions namespace requirement for using dialogs
    public class DialogParameters : NavigationParameters, IDialogParameters
    {
        public DialogParameters(string query) : base(query)
        {
        }
    }
}
