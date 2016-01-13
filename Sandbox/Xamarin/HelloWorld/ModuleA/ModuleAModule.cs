using Microsoft.Practices.Unity;
using ModuleA.Views;
using Prism.Modularity;
using Prism.Unity;

namespace ModuleA
{
    public class ModuleAModule : IModule
    {
        readonly IUnityContainer _container;

        public ModuleAModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterTypeForNavigation<ViewA>();
            _container.RegisterTypeForNavigation<ViewB>();
            _container.RegisterTypeForNavigation<ViewC>();
        }
    }
}
