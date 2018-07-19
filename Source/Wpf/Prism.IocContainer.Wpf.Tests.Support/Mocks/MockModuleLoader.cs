

using Prism.Modularity;

namespace Prism.IocContainer.Wpf.Tests.Support.Mocks
{
    public class MockModuleInitializer : IModuleInitializer
    {
        public bool LoadCalled;

        public void Initialize(IModuleInfo moduleInfo)
        {
            LoadCalled = true;
        }
    }
}