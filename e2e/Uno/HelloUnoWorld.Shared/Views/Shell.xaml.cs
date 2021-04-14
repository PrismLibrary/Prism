using System.Windows;


#if HAS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace HelloUnoWorld.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : ContentControl
    {
        public Shell()
        {
            InitializeComponent();
        }
    }
}
