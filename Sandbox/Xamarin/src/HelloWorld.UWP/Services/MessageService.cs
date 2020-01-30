using HelloWorld.Interfaces;

namespace HelloWorld.UWP.Services
{
    public class MessageService : IMessageService
    {
        public string Message => "Hello from UWP";
    }
}
