using Prism.Ioc;
using Prism.Modularity;

namespace Prism.DI.Forms.Tests.Mocks.Modules
{
    public class ModuleMock : IModule
    {
        public bool Initialized { get; private set; }
        public bool RegisterTypesCalled { get; private set; }

        public void OnInitialize()
        {
            Initialized = true;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterTypesCalled = true;
        }
    }
}
