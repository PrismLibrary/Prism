using System;
using Prism.Modularity;
using Autofac;

namespace Prism.Autofac.Forms.Modularity
{
    public class AutofacModuleInitializer : IModuleInitializer
    {
        readonly IContainer _container;
        public AutofacModuleInitializer(IContainer context)
        {
            _container = context;
        }

        public void Initialize(ModuleInfo moduleInfo)
        {
            var module = (IModule)_container.Resolve(moduleInfo.ModuleType);
            if (module != null)
                module.Initialize();
        }
    }
}
