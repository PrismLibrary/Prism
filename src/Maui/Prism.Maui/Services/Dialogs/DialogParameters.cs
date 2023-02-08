using Prism.Common;

namespace Prism.Services;

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
