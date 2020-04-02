using HelloWorld.Droid.Services;
using HelloWorld.Interfaces;
using Prism;
using Prism.Ioc;

namespace HelloWorld.Droid
{
    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
        }
    }
}