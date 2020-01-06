using System;
using Prism.Ioc;
using Prism.Modularity;

namespace Prism.Forms.Tests.Mocks.Modules
{
    public class BadModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            throw new Exception(nameof(OnInitialized));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new Exception(nameof(RegisterTypes));
        }
    }
}
