using System;
using Prism.Modularity;
using Autofac;

namespace Prism.Autofac.Modularity
{
    public class AutofacModuleInitializer : IModuleInitializer
    {
        readonly IComponentContext _context;

        /// <summary>
        /// Create a new instance of <see cref="AutofacModuleInitializer"/> with <paramref name="context"/>
        /// </summary>
        /// <param name="context"></param>
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

        /// <summary>
        /// Create the <see cref="IModule"/> for <paramref name="moduleType"/> by resolving from <see cref="_context"/>
        /// </summary>
        /// <param name="moduleType">Type of module to create</param>
        /// <returns>An isntance of <see cref="IModule"/> for <paramref name="moduleType"/> if exists; otherwise <see langword="null" /></returns>
        protected virtual IModule CreateModule(Type moduleType)
        {
            return _context.Resolve(moduleType) as IModule;
        }
    }
}
