using Prism.Navigation;

namespace Prism.Services.Dialogs
{
    public class DialogParameters : NavigationParameters, IDialogParameters
    {
        public DialogParameters()
        {
        }

        public DialogParameters(string query) : base(query)
        {
        }

        internal DialogParameters(INavigationParameters parameters, NavigationMode mode)
        {
            foreach(var param in parameters)
            {
                Add(param.Key, param.Value);
            }
            this.AddNavigationMode(mode);
        }
    }
}