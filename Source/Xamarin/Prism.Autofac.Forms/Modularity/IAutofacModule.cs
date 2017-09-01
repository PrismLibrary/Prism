using Autofac;
using Prism.Modularity;

namespace Prism.Autofac.Modularity
{
    /// <summary>
    /// Autofac Module.
    /// </summary>
    public interface IAutofacModule : IModule
    {
        /// <summary>
        /// Initialize using the specified builder.
        /// </summary>
        /// <param name="builder">Builder.</param>
        void Initialize(ContainerBuilder builder);
    }
}
