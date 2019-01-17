using System.Windows;

namespace Prism.Services.Dialogs.DefaultDialogs
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window, IDialogWindow
    {
        public IDialog ViewModel
        {
            get { return (IDialog)DataContext; }
            set { DataContext = value; }
        }

        public DialogWindow()
        {
            InitializeComponent();
        }
    }
}
