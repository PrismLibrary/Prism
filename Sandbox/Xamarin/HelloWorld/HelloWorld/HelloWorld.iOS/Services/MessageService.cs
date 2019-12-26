using HelloWorld.Interfaces;

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
