using Prism.Ioc;
using Prism.Modularity;
using System;

namespace Prism.Wpf.Tests.Mocks.Modules
{
    [Module(ModuleName = "DependantModule")]
    [ModuleDependency("DependencyModule")]
    public class DependantModule : IModule
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
