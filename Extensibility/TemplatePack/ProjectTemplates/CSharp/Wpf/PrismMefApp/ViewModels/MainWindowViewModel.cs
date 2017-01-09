using System.ComponentModel.Composition;
using Prism.Mvvm;

namespace $safeprojectname$.ViewModels
{
    [Export(typeof(MainWindowViewModel))]
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Mef Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
