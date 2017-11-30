using Prism.Ioc;
using Prism.Modularity;
using System;

namespace Prism.Wpf.Tests.Mocks.Modules
{
    public abstract class MockAbstractModule : IModule
    {
        public void Initialize()
        {
        }

        public void OnInitialized()
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }

    public class MockInheritingModule : MockAbstractModule
    {
    }
}
