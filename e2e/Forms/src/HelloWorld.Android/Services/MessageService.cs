using HelloWorld.Droid.Services;
using HelloWorld.Interfaces;

namespace HelloWorld.Droid.Services
{
    public class MessageService : IMessageService
    {
        public string Message
        {
            get { return "Hello from Android"; }
        }
    }
}