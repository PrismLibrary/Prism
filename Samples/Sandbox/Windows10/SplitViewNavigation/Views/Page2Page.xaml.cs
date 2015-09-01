using System.Diagnostics;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SplitViewNavigation.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page2Page : Page
    {
        public Page2Page()
        {
            Debug.WriteLine("Page2Page()");
            this.InitializeComponent();
        }
    }
}
