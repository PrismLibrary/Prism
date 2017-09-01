using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Modularity;

namespace Prism.Autofac.Modularity
{
    /// <summary>
    /// Autofac module manager.
    /// </summary>
    public class AutofacModuleManager : ModuleManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Prism.Autofac.Modularity.AutofacModuleManager"/> class.
        /// </summary>
        /// <param name="moduleInitializer">Module initializer.</param>
        /// <param name="moduleCatalog">Module catalog.</param>
        public AutofacModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog) 
            : base(moduleInitializer, moduleCatalog)
        {
        }

        /// <summary>
        /// Loads the <see cref="IModule"/> 
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Throw exception if trying to load modules OnDemand, as this is an unsupported feature with Autofac
        /// </exception>
        /// <param name="moduleInfos">Module infos.</param>
        protected override void LoadModules(IEnumerable<ModuleInfo> moduleInfos)
        {
            if (moduleInfos.Any(info => info.InitializationMode == InitializationMode.OnDemand))
                throw new NotSupportedException("Autofac does not support OnDemand Modules.");
            
            base.LoadModules(moduleInfos);
        }
    }
}
