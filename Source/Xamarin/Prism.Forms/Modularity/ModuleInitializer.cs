using Prism.Ioc;
using System;

namespace Prism.Modularity
{
    public class ModuleInitializer : IModuleInitializer
    {
        readonly IContainer _container;

        public ModuleInitializer(IContainer container)
        {
            _container = container;
        }

        public void Initialize(ModuleInfo moduleInfo)
        {
            var module = CreateModule(moduleInfo.ModuleType);
            if (module != null)
                module.Initialize();
        }

        protected virtual IModule CreateModule(Type moduleType)
        {
            return (IModule)_container.Resolve(moduleType);
        }
    }
}
