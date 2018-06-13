using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism;
using Foundation;
using UIKit;
using Prism.Ioc;
using HelloWorld.Interfaces;
using HelloWorld.iOS.Services;

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