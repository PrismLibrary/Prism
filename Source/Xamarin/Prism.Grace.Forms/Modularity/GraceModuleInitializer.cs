using System;
using Prism.Modularity;
using Grace.DependencyInjection;

namespace Prism.Grace.Modularity
{
    public class GraceModuleInitializer : IModuleInitializer
    {
        private readonly DependencyInjectionContainer _container;

        /// <summary>
        /// Create a new instance of <see cref="GraceModuleInitializer"/> with <paramref name="container"/>
        /// </summary>
        /// <param name="container"></param>
        public GraceModuleInitializer(DependencyInjectionContainer container)
        {
            _container = container;
        }

        public void Initialize(ModuleInfo moduleInfo)
        {
            var module = CreateModule(moduleInfo.ModuleType);
            module?.Initialize();
        }

        /// <summary>
        /// Create the <see cref="IModule"/> for <paramref name="moduleType"/> by resolving from <see cref="_container"/>
        /// </summary>
        /// <param name="moduleType">Type of module to create</param>
        /// <returns>An instance of <see cref="IModule"/> for <paramref name="moduleType"/> if exists; otherwise <see langword="null" /></returns>
        protected virtual IModule CreateModule(Type moduleType)
        {
            return _container.Locate(moduleType) as IModule;
        }
    }
}