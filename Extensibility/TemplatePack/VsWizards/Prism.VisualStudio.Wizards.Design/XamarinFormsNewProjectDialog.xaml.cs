using System.Windows;

namespace Prism.VisualStudio.Wizards.Design
{
    /// <summary>
    /// Interaction logic for XamarinFormsNewProjectDialog.xaml
    /// </summary>
    public partial class XamarinFormsNewProjectDialog : Window
    {
        public XamarinFormsNewProjectDialogResult Result { get; set; }


        public XamarinFormsNewProjectDialog()
        {
            InitializeComponent();
            Result = new XamarinFormsNewProjectDialogResult();
            Closed += XamarinFormsNewProjectDialog_Closed;
        }

        private void XamarinFormsNewProjectDialog_Closed(object sender, System.EventArgs e)
        {
            Result.CreateAndroid = _chkAndroid.IsChecked.HasValue ? _chkAndroid.IsChecked.Value : false;
            Result.CreateiOS = _chkiOS.IsChecked.HasValue ? _chkiOS.IsChecked.Value : false;
            Result.CreateUwp = _chkUwp.IsChecked.HasValue ? _chkUwp.IsChecked.Value : false;
            Result.CreateWinStore = _chkWinStore.IsChecked.HasValue ? _chkWinStore.IsChecked.Value : false;
            Result.CreateWinPhone = _chkWinPhone.IsChecked.HasValue ? _chkWinPhone.IsChecked.Value : false;
        }

        private void _btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _btnClose_Click(object sender, RoutedEventArgs e)
        {
            Result.Cancelled = true;
            Close();
        }
    }

    public class XamarinFormsNewProjectDialogResult
    {
        public bool CreateAndroid { get; set; }

        public bool CreateiOS { get; set; }

        public bool CreateUwp { get; set; }

        public bool CreateWinStore { get; set; }

        public bool CreateWinPhone { get; set; }

        public bool Cancelled { get; set; }
    }
}
