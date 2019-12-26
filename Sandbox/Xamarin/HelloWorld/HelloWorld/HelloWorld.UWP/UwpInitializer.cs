using HelloWorld.Interfaces;
using HelloWorld.UWP.Services;
using Prism;
using Prism.Ioc;

namespace HelloWorld.UWP
{
    public class UwpInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
        }
    }
}
