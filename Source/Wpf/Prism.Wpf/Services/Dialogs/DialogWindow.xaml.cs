using System.Windows;

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window, IDialogWindow
    {
        public IDialogResult Result { get; set; }

        public DialogWindow()
        {
            InitializeComponent();
        }
    }
}
