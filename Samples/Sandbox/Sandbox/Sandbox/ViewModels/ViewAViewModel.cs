using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace Sandbox.ViewModels
{
    public class ViewAViewModel : NavigationViewModelBase
    {
        public ICommand ClickCommand { get; set; }

        public ViewAViewModel()
        {
            ClickCommand = new DelegateCommand(Click);
        }

        private void Click()
        {
            Navigate("ViewC");
        }
    }
}
