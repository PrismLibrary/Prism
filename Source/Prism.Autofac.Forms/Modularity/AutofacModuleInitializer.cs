using System;
using Prism.Modularity;
using Autofac;

namespace Prism.Autofac.Forms.Modularity
{
    public class AutofacModuleInitializer : IModuleInitializer
    {
        readonly IComponentContext _context;
        public AutofacModuleInitializer(IComponentContext context)
        {
            _context = context;
        }

        public void Initialize(ModuleInfo moduleInfo)
        {
            var module = (IModule)_context.Resolve(moduleInfo.ModuleType);
            if (module != null)
                module.Initialize();
        }
    }
}
