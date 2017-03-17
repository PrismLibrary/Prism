

using Prism.Modularity;

namespace Prism.Wpf.Tests.Mocks.Modules
{
    public class MockModuleThrowingException : IModule
    {
        public void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}
