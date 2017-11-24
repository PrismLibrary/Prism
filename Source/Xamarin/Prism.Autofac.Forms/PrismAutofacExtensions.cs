using Autofac;
using Prism.Ioc;

namespace Prism.Autofac.Forms
{
    public static class PrismAutofacExtensions
    {
        /// <summary>
        /// Gets the <see cref="ContainerBuilder"/> used to register services
        /// </summary>
        /// <param name="registry"></param>
        /// <returns>The current <see cref="ContainerBuilder"/></returns>
        public static ContainerBuilder Builder(this IContainerRegistry registry) =>
            ((IAutofacContainerExtension)registry).Builder;
    }
}
