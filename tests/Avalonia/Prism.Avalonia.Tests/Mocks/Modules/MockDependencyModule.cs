using System;
using Prism.Ioc;
using Prism.Modularity;

namespace Prism.Avalonia.Tests.Mocks.Modules
{
    [Module(ModuleName = "DependencyModule")]
    public class DependencyModule : IModule
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
