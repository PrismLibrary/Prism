using Prism.Dialogs;

namespace Prism.Commands.Parameters;

/// <summary>
/// The ShowDialogCommand parameters
/// </summary>
public class ShowDialogCommandParameter
{
    /// <summary>
    /// The name of dialog
    /// </summary>
    public string DialogName { get; set; }

    /// <summary>
    /// The dialog parameters
    /// </summary>
    public DialogParameters Parameters { get; set; }
}
