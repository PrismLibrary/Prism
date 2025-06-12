using System;
using Prism.Ioc;
using Prism.Modularity;

namespace Prism.Avalonia.Tests.Mocks.Modules
{
    public class MockModuleThrowingException : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            throw new NotImplementedException();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new NotImplementedException();
        }
    }
}
