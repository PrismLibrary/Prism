using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

#nullable enable
namespace Prism.Dialogs
{
    /// <summary>
    /// Prism's default dialog host.
    /// </summary>
    public partial class DialogWindow : ContentDialog, IDialogWindow
    {
        public DialogWindow()
        {
            this.InitializeComponent();
        }
        public IDialogResult? Result { get; set; }

        event RoutedEventHandler IDialogWindow.Loaded
        {
            add => Loaded += value;
            remove => Loaded -= value;
        }

        event TypedEventHandler<ContentDialog, ContentDialogClosingEventArgs> IDialogWindow.Closing
        {
            add => Closing += value;
            remove => Closing -= value;
        }

        event TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs> IDialogWindow.Closed
        {
            add => Closed += value;
            remove => Closed -= value;
        }
    }
}
