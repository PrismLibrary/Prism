using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Prism.Services.Dialogs
{
    public interface IDialogWindow
    {
        object DataContext { get; set; }

        Style Style { get; set; }

        event RoutedEventHandler Loaded;

        event TypedEventHandler<ContentDialog, ContentDialogClosingEventArgs> Closing;
        event TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs> Closed;

        IDialogResult Result { get; set; }

        object Content { get; set; }

        IAsyncOperation<ContentDialogResult> ShowAsync();

        void Hide();
    }
}
