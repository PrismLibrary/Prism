using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Sandbox.Core;

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
            var parameters = new NavigationParameters();
            parameters.Add("Message", "A message from ViewA");

            //when navigating within a NavigationPage, set useModalNavigation = false
            Navigate("ViewCKey", parameters);
        }
    }
}
