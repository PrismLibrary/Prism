using Prism.Commands;
using Prism.Events;
using Sandbox.Core;
using Sandbox.Events;
using Sandbox.Services;

namespace Sandbox.ViewModels
{
    public class ViewBViewModel : NavigationViewModelBase
    {
        /// <summary>
        /// If ITextToSpeechService isn't registered with the UnityContainer (which it's not in this sample), Prism will automatically look for it in the Xamarin.Forms DependenyService.
        /// This removes the need to manually call the static DependencyService.Get method and keeps your ViewModel testable"/>
        /// </summary>
        private readonly ITextToSpeechService _textToSpeechService;

        private string _message = "View B";
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public DelegateCommand SpeakCommand { get; set; }

        public ViewBViewModel(IEventAggregator eventAggregator, ITextToSpeechService textToSpeechService)
        {
            _textToSpeechService = textToSpeechService;

            eventAggregator.GetEvent<MessageSentEvent>().Subscribe(MessageSent);

            SpeakCommand = new DelegateCommand(Speak);
        }

        private void Speak()
        {
            _textToSpeechService.Speak(Message);
        }

        private void MessageSent(string message)
        {
            Message = message;
        }
    }
}
