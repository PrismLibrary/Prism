using Prism.Common;

namespace Prism.Dialogs;

/// <summary>
/// Provides a base implementation of <see cref="IDialogParameters"/>.
/// </summary>
public class DialogParameters : ParametersBase, IDialogParameters
{
    /// <summary>
    /// Initializes a new instance of <see cref="DialogParameters"/>.
    /// </summary>
    public DialogParameters()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogParameters"/> based on a specified query string.
    /// </summary>
    /// <param name="query">A uri query string</param>
    public DialogParameters(string query)
        : base(query)
    {
    }
}
