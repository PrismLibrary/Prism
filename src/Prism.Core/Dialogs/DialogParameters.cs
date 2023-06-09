using Prism.Common;

namespace Prism.Dialogs;

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
