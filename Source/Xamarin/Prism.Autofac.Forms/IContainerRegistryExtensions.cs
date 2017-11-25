using Autofac;
using Prism.Ioc;

namespace Prism.Autofac
{
    public static class IContainerRegistryExtensions
    {
        /// <summary>
        /// Gets the <see cref="ContainerBuilder"/> used to register services.
        /// </summary>
        /// <param name="registry"></param>
        /// <returns>The current <see cref="ContainerBuilder"/></returns>
        public static ContainerBuilder GetBuilder(this IContainerRegistry registry)
        {
            return ((IAutofacContainerExtension)registry).Builder;
        }
    }
}
