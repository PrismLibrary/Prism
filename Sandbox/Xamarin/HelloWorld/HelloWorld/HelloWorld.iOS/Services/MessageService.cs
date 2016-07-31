using HelloWorld.Interfaces;
using HelloWorld.iOS.Services;

[assembly: Xamarin.Forms.Dependency(typeof(MessageService))]
namespace HelloWorld.iOS.Services
{
    public class MessageService : IMessageService
    {
        public string Message
        {
            get { return "Hello from iOS"; }
        }
    }
}
