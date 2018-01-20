using Prism.Ioc;
using System;

namespace Prism.Modularity
{
    public class ModuleInitializer : IModuleInitializer
    {
        readonly IContainerExtension _container;

        public ModuleInitializer(IContainerExtension container)
        {
            _container = container;
        }

        public void Initialize(ModuleInfo moduleInfo)
        {
            var module = CreateModule(moduleInfo.ModuleType);
            if (module != null)
            {
                module.RegisterTypes(_container);
                module.OnInitialized(_container);
            }
        }

        protected virtual IModule CreateModule(Type moduleType)
        {
            return (IModule)_container.Resolve(moduleType);
        }
    }
}
