using System.Windows;

namespace Prism.Services.Dialogs.DefaultDialogs
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class RegionAwareDialogWindow: Window, IRegionAwareDialogWindow
    {
        public IDialogResult Result { get; set; }

        public DialogWindow()
        {
            InitializeComponent();
        }
    }
}
