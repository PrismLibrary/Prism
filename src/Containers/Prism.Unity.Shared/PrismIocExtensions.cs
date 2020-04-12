using Prism.Ioc;
using Unity;

namespace Prism.Unity
{
    /// <summary>
    /// Extensions help get the underlying <see cref="IUnityContainer" />
    /// </summary>
    public static class PrismIocExtensions
    {
        /// <summary>
        /// Gets the <see cref="IUnityContainer" /> from the <see cref="IContainerProvider" />
        /// </summary>
        /// <param name="containerProvider">The current <see cref="IContainerProvider" /></param>
        /// <returns>The underlying <see cref="IUnityContainer" /></returns>
        public static IUnityContainer GetContainer(this IContainerProvider containerProvider)
        {
            return ((IContainerExtension<IUnityContainer>)containerProvider).Instance;
        }

        /// <summary>
        /// Gets the <see cref="IUnityContainer" /> from the <see cref="IContainerProvider" />
        /// </summary>
        /// <param name="containerRegistry">The current <see cref="IContainerRegistry" /></param>
        /// <returns>The underlying <see cref="IUnityContainer" /></returns>
        public static IUnityContainer GetContainer(this IContainerRegistry containerRegistry)
        {
            return ((IContainerExtension<IUnityContainer>)containerRegistry).Instance;
        }
    }
}
