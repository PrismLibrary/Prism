using HelloWorld.WinPhone.Services;
using HelloWorld.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(MessageService))]
namespace HelloWorld.WinPhone.Services
{
    public class MessageService : IMessageService
    {
        public string Message
        {
            get { return "Hello from WinPhone"; }
        }
    }
}
