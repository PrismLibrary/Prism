using Autofac;
using Prism.Ioc;

namespace Prism.Autofac.Ioc
{
    /// <summary>
    /// Provides customization extensions for Autofac Applications 
    /// </summary>
    public interface IAutofacContainerExtension : IContainerExtension<IContainer>
    {
        /// <summary>
        /// Gets the <see cref="ContainerBuilder"/> used to register the services for the application
        /// </summary>
        ContainerBuilder Builder { get; }
    }
}
