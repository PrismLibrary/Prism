using System;
using Prism.Modularity;
using Autofac;

namespace Prism.Autofac.Forms.Modularity
{
    public class AutofacModuleInitializer : IModuleInitializer
    {
        readonly IContainer _container;

        /// <summary>
        /// Create a new instance of <see cref="AutofacModuleInitializer"/> with <paramref name="container"/>
        /// </summary>
        /// <param name="container"></param>
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

        /// <summary>
        /// Create the <see cref="IModule"/> for <paramref name="moduleType"/> by resolving from <see cref="_container"/>
        /// </summary>
        /// <param name="moduleType">Type of module to create</param>
        /// <returns>An isntance of <see cref="IModule"/> for <paramref name="moduleType"/> if exists; otherwise <see langword="null" /></returns>
        protected virtual IModule CreateModule(Type moduleType)
        {
            return _container.Resolve(moduleType) as IModule;
        }
    }
}
