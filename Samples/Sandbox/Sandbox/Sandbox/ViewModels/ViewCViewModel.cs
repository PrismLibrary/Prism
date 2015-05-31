using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Sandbox.Core;
using Sandbox.Events;

namespace Sandbox.ViewModels
{
    public class ViewCViewModel : NavigationViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;

        public ICommand ClickCommand { get; set; }

        private string _messageToSend;
        public string MessageToSend
        {
            get { return _messageToSend; }
            set { SetProperty(ref _messageToSend, value); }
        }

        public ViewCViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
        {
            NavigationService = navigationService;
            _eventAggregator = eventAggregator;
            ClickCommand = new DelegateCommand<string>(Click);
        }

        private void Click(string message)
        {
            _eventAggregator.GetEvent<MessageSentEvent>().Publish(message);

            //when navigating within a NavigationPage, set useModalNavigation = false
            NavigationService.GoBack(); 
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            MessageToSend = parameters["Message"].ToString();
        }
    }
}
