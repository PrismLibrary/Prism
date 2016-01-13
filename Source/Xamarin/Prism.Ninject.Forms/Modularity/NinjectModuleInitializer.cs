using System;
using Prism.Modularity;
using Ninject;

namespace Prism.Ninject.Modularity
{
    public class NinjectModuleInitializer : IModuleInitializer
    {
        readonly IKernel _kernel;

        public NinjectModuleInitializer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Initialize(ModuleInfo moduleInfo)
        {
            var module = CreateModule(moduleInfo.ModuleType);
            if (module != null)
                module.Initialize();
        }

        protected virtual IModule CreateModule(Type moduleType)
        {
            return (IModule)_kernel.Get(moduleType);
        }
    }
}
