using System;
using Prism.Modularity;
using Autofac;

namespace Prism.Autofac.Modularity
{
    /// <summary>
    /// Autofac Module Initializer.
    /// </summary>
    public class AutofacModuleInitializer : IModuleInitializer
    {
        readonly ContainerBuilder _builder;

        /// <summary>
        /// Create a new instance of <see cref="AutofacModuleInitializer"/> with <paramref name="builder"/>
        /// </summary>
        /// <param name="builder"></param>
        public AutofacModuleInitializer(ContainerBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Initialize the specified Module.
        /// </summary>
        /// <param name="moduleInfo">Module info.</param>
        public void Initialize(ModuleInfo moduleInfo) =>
            CreateModule(moduleInfo.ModuleType)?.Initialize(_builder);

        /// <summary>
        /// Uses the <see cref="Activator"/> to create an instance of your <see cref="IAutofacModule"/> 
        /// </summary>
        /// <param name="moduleType">Type of module to create</param>
        /// <returns>An instance of <see cref="IAutofacModule"/> for <paramref name="moduleType"/></returns>
        protected virtual IAutofacModule CreateModule(Type moduleType) =>
            (IAutofacModule)Activator.CreateInstance(moduleType);
    }
}
