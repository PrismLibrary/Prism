using Prism.Services.Dialogs;
using System.Windows;

namespace HelloWorld.Dialogs
{
    /// <summary>
    /// Interaction logic for AnotherDialogWindow.xaml
    /// </summary>
    public partial class AnotherDialogWindow : Window, IDialogWindow
    {
        public AnotherDialogWindow()
        {
            InitializeComponent();
        }

        public IDialogResult Result { get; set; }
    }
}
