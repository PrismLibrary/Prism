using HelloWorld.Interfaces;
using HelloWorld.iOS.Services;
using Prism;
using Prism.Ioc;

namespace HelloWorld.iOS
{
    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
        }
    }
}