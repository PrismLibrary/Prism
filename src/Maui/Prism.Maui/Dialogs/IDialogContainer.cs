using System.Windows.Input;

#nullable enable
namespace Prism.Dialogs;

/// <summary>
/// Interface representing a container for managing dialogs.
/// </summary>
public interface IDialogContainer
{
    /// <summary>
    /// Gets a stack of currently active dialog containers.
    /// </summary>
    /// <remarks>
    /// This property provides access to the list of dialogs currently displayed, typically in the order they were presented.
    /// </remarks>
    static IList<IDialogContainer> DialogStack { get; } = [];

    /// <summary>
    /// Gets the view associated with the currently displayed dialog.
    /// </summary>
    View DialogView { get; }

    /// <summary>
    /// Gets a command that can be used to dismiss the currently displayed dialog.
    /// </summary>
    ICommand Dismiss { get; }

    /// <summary>
    /// Configures the layout and behavior of the dialog.
    /// </summary>
    /// <param name="currentPage">The page on which the dialog is being displayed.</param>
    /// <param name="dialogView">The view representing the dialog content.</param>
    /// <param name="hideOnBackgroundTapped">True if the dialog should close when the background is tapped, false otherwise.</param>
    /// <param name="dismissCommand">The command to execute when the dialog is dismissed.</param>
    /// <param name="parameters">Optional parameters to pass to the dialog.</param>
    /// <returns>A task representing the completion of the configuration process.</returns>
    Task ConfigureLayout(Page currentPage, View dialogView, bool hideOnBackgroundTapped, ICommand dismissCommand, IDialogParameters parameters);

    /// <summary>
    /// Performs the internal logic to display or dismiss the dialog.
    /// </summary>
    /// <param name="currentPage">The page on which the dialog is being displayed.</param>
    /// <returns>A task representing the completion of the pop operation.</returns>
    Task DoPop(Page currentPage);
}
