using System.Windows;

namespace Prism.Services.Dialogs.DefaultDialogs
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window, IDialogWindow
    {
        public IDialogAware ViewModel
        {
            get { return (IDialogAware)DataContext; }
            set { DataContext = value; }
        }

        public IDialogResult Result { get; set; }

        public DialogWindow()
        {
            InitializeComponent();
        }
    }
}
