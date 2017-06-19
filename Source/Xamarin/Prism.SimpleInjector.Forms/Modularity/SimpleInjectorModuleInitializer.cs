using System;
using SimpleInjector;
using Prism.Modularity;

namespace Prism.SimpleInjector.Modularity
{
    public class SimpleInjectorModuleInitializer : IModuleInitializer
    {
        private readonly Container _container;

        /// <summary>
        /// Create a new instance of <see cref="SimpleInjectorModuleInitializer"/> with <paramref name="container"/>
        /// </summary>
        /// <param name="container"></param>
        public SimpleInjectorModuleInitializer(Container container)
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
        /// <returns>An isntance of <see cref="IModule"/> for <paramref name="moduleType"/> if exists; otherwise <see langword="null" /></returns>
        protected virtual IModule CreateModule(Type moduleType)
        {
            return _container.GetInstance(moduleType) as IModule;
        }
    }
}