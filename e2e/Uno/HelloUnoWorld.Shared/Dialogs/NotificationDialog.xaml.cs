#if HAS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace HelloUnoWorld.Dialogs
{
    /// <summary>
    /// Interaction logic for NotificationDialog.xaml
    /// </summary>
    public partial class NotificationDialog : ContentControl
    {
        public NotificationDialog()
        {
            InitializeComponent();
        }
    }
}
