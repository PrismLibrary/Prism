using DryIoc;
using Prism.Ioc;

namespace Prism.DryIoc
{
    /// <summary>
    /// Extensions help get the underlying <see cref="IContainer" />
    /// </summary>
    public static class PrismIocExtensions
    {
        /// <summary>
        /// Gets the <see cref="IContainer" /> from the <see cref="IContainerProvider" />
        /// </summary>
        /// <param name="containerProvider">The current <see cref="IContainerProvider" /></param>
        /// <returns>The underlying <see cref="IContainer" /></returns>
        public static IContainer GetContainer(this IContainerProvider containerProvider)
        {
            return ((IContainerExtension<IContainer>)containerProvider).Instance;
        }

        /// <summary>
        /// Gets the <see cref="IContainer" /> from the <see cref="IContainerProvider" />
        /// </summary>
        /// <param name="containerRegistry">The current <see cref="IContainerRegistry" /></param>
        /// <returns>The underlying <see cref="IContainer" /></returns>
        public static IContainer GetContainer(this IContainerRegistry containerRegistry)
        {
            return ((IContainerExtension<IContainer>)containerRegistry).Instance;
        }
    }
}
