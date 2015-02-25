using Prism.Events;
using Prism.Mvvm;
using Sandbox.Events;

namespace Sandbox.ViewModels
{
    public class ViewBViewModel : NavigationViewModelBase
    {
        private string _message;

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public ViewBViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<MessageSentEvent>().Subscribe(MessageSent);
        }

        private void MessageSent(string message)
        {
            Message = message;
        }
    }
}
