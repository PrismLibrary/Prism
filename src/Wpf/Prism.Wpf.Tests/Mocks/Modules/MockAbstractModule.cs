

using Prism.Modularity;

namespace Prism.Wpf.Tests.Mocks.Modules
{
    public abstract class MockAbstractModule : IModule
    {
        public void Initialize()
        {
        }
    }

    public class MockInheritingModule : MockAbstractModule
    {
    }
}
