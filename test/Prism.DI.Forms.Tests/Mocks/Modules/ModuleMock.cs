using Prism.Modularity;

namespace Prism.DI.Forms.Tests.Mocks.Modules
{
    public class ModuleMock : IModule
    {
        public bool Initialized { get; private set; }

        public void Initialize()
        {
            Initialized = true;
        }
    }
}
