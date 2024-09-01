using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Styling;

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
        /// <remarks>Avalonia's WindowBase.Owner's property access is { get; protected set; }.</remarks>
        WindowBase Owner { get; }

        /// <summary>Show a non-modal dialog.</summary>
        void Show();

        /// <summary>Show a modal dialog.</summary>
        /// <returns></returns>
        Task ShowDialog(Window owner);

        /// <summary>
        /// The data context of the window.
        /// </summary>
        /// <remarks>
        /// The data context must implement <see cref="IDialogAware"/>.
        /// </remarks>
        object DataContext { get; set; }

        /// <summary>Called when the window is loaded.</summary>
        /// <remarks>
        ///     Avalonia currently doesn't implement the Loaded event like WPF.
        ///     Window > WindowBase > TopLevel.Opened
        ///     Window > WindowBase > TopLevel > Control > InputElement > Interactive > layout > Visual > StyledElement.Initialized
        /// </remarks>
        //// WPF: event RoutedEventHandler Loaded;
        event EventHandler Opened;

        /// <summary>
        /// Called when the window is closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        // WPF: event CancelEventHandler Closing;
        // Ava: ...
        event EventHandler<WindowClosingEventArgs>? Closing;

        /// <summary>
        /// The result of the dialog.
        /// </summary>
        IDialogResult Result { get; set; }

        /// <summary>The window style.</summary>
        // WPF: Window > ContentControl > FrameworkElement
        // Ava: Window > WindowBase > TopLevel > ContentControl > TemplatedControl > Control
        //Style Style { get; set; }
    }
}
