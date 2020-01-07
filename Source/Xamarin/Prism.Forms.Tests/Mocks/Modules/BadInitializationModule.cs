using System;
using Prism.Ioc;
using Prism.Modularity;

namespace Prism.Forms.Tests.Mocks.Modules
{
    public class BadInitializationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            throw new Exception(nameof(OnInitialized));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
