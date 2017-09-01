using System;
using Autofac;

namespace Prism.Autofac.Modularity
{
    /// <summary>
    /// Autofac Module Base.
    /// </summary>
    public abstract class AutofacModuleBase : IAutofacModule
    {
        /// <summary>
        /// Initializes the Module using the <see cref="PrismApplication"/>'s <see cref="ContainerBuilder"/>.
        /// </summary>
        /// <returns>The initialize.</returns>
        /// <param name="builder">Builder.</param>
        public abstract void Initialize(ContainerBuilder builder);

        /// <summary>
        /// Implements the base <see cref="Prism.Modularity.IModule.Initialize"/> which is
        /// unsupported in Autofac
        /// </summary>
        public void Initialize()
        {
            throw new NotSupportedException();
        }
    }
}
