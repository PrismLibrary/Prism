using Autofac;
using Prism.Autofac.Ioc;
using Prism.Ioc;

namespace Prism.Autofac
{
    public static class PrismIocExtensions
    {
        public static IContainer GetContainer(this IContainerProvider containerProvider)
        {
            return ((IContainerExtension<IContainer>)containerProvider).Instance;
        }

        public static IContainer GetContainer(this IContainerRegistry containerRegistry)
        {
            return ((IContainerExtension<IContainer>)containerRegistry).Instance;
        }

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
