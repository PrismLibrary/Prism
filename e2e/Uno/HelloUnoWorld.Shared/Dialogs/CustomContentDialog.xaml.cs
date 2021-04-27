using Prism.Services.Dialogs;
using Windows.Foundation;

#if HAS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif


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

#if HAS_UNO_WINUI || NETCOREAPP
            // This is needed to enable dialog to be created properly.
            // See: https://github.com/microsoft/microsoft-ui-xaml/issues/4251
            XamlRoot = App.MainXamlRoot;
#endif
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
