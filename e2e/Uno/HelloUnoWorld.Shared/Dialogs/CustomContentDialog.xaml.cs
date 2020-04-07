using Prism.Services.Dialogs;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelloUnoWorld.Dialogs
{
    /// <summary>
    /// Interaction logic for CustomDialogWindow.xaml
    /// </summary>
    public partial class CustomContentDialog : ContentDialog, IDialogWindow
    {
        public CustomContentDialog()
        {
            InitializeComponent();
        }

        public IDialogResult Result { get ; set; }

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
