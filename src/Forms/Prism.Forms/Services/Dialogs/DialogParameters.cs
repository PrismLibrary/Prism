using Prism.Common;

namespace Prism.Services.Dialogs
{
    public class DialogParameters : ParametersBase, IDialogParameters
    {
        public DialogParameters()
        {
        }

        public DialogParameters(string query)
            : base(query)
        {
        }
    }
}
