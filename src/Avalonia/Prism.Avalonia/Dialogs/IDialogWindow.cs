using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Styling;

#nullable enable
namespace Prism.Dialogs
{
    /// <summary>
    /// Interface for a dialog hosting window.
    /// </summary>
    public interface IDialogWindow
    {
        /// <summary>Dialog content.</summary>
        object Content { get; set; }

        /// <summary>Close the window.</summary>
        void Close();

        /// <summary>The window's owner.</summary>
        /// <remarks>The owner is read-only through this contract. Supply it when showing a modal dialog.</remarks>
        WindowBase Owner { get; }

        /// <summary>Show a non-modal dialog.</summary>
        void Show();

        /// <summary>Show a modal dialog.</summary>
        /// <returns>Task.</returns>
        Task ShowDialog(Window owner);

        /// <summary>The data context of the window.</summary>
        /// <remarks>The data context must implement <see cref="IDialogAware"/>.</remarks>
        object DataContext { get; set; }

        /// <summary>Raised when the dialog window is opened.</summary>
        event EventHandler Opened;

        /// <summary>
        /// Called when the window is closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>Called when the window is closing.</summary>
        event EventHandler<WindowClosingEventArgs>? Closing;

        /// <summary>The result of the dialog.</summary>
        IDialogResult Result { get; set; }

    }
}
