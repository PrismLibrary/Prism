using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Sandbox.Events;

namespace Sandbox.ViewModels
{
    public class ViewCViewModel : NavigationViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;

        public ICommand ClickCommand { get; set; }

        public ViewCViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            ClickCommand = new DelegateCommand<string>(Click);
        }

        private void Click(string message)
        {
            _eventAggregator.GetEvent<MessageSentEvent>().Publish(message);

            //when navigating within a NavigationPage, set useModalNavigation = false
            GoBack(useModalNavigation:false); 
        }
    }
}
