using Prism.Ioc;
using Prism.Modularity;

namespace Prism.Forms.Tests.Mocks.Modules
{
    [ModuleDependency(nameof(DependentModuleA))]
    [ModuleDependency(nameof(DependentModuleB))]
    public class MasterModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
