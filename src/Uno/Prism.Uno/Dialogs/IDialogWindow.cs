using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

#nullable enable
namespace Prism.Dialogs
{
    /// <summary>Defines a host for Prism dialog content and its close lifecycle.</summary>
    public interface IDialogWindow
    {
        /// <summary>Gets or sets the dialog view model.</summary>
        object? DataContext { get; set; }

        /// <summary>Gets or sets the style applied to the dialog host.</summary>
        Style Style { get; set; }

        /// <summary>Occurs when the dialog host is loaded into the visual tree.</summary>
        event RoutedEventHandler Loaded;

        /// <summary>Occurs before the dialog closes, allowing closure to be cancelled.</summary>
        event TypedEventHandler<ContentDialog, ContentDialogClosingEventArgs> Closing;
        /// <summary>Occurs after the dialog has closed.</summary>
        event TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs> Closed;

        /// <summary>Gets or sets the result supplied when the dialog closes.</summary>
        IDialogResult? Result { get; set; }

        /// <summary>Gets or sets the view displayed inside the dialog host.</summary>
        object? Content { get; set; }

        /// <summary>Displays the dialog asynchronously at the specified placement.</summary>
        /// <param name="placement">Where the content dialog is displayed.</param>
        /// <returns>An operation completing with the content dialog result when the dialog closes.</returns>
        IAsyncOperation<ContentDialogResult> ShowAsync(ContentDialogPlacement placement);

        /// <summary>Closes the displayed dialog.</summary>
        void Hide();
    }
}
