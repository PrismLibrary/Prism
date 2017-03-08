

using Prism.Modularity;

namespace Prism.Wpf.Tests.Mocks.Modules
{
    public class MockModuleReferencingAssembly : IModule
    {
        public void Initialize()
        {
            MockReferencedModule instance = new MockReferencedModule();
        }
    }
}