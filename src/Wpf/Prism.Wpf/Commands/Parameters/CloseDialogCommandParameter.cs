using Prism.Dialogs;

namespace Prism.Commands.Parameters;

/// <summary>
/// The CloseDialogCommand parameters
/// </summary>
public class CloseDialogCommandParameter
{
    /// <summary>
    /// The dialog result
    /// </summary>
    public ButtonResult ButtonResult { get; set; }

    /// <summary>
    /// The dialog DataContext
    /// </summary>
    public IDialogAware DialogContext { get; set; }
}
