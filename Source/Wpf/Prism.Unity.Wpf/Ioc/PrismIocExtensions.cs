using Prism.Ioc;
using Unity;

namespace Prism.Unity
{
    public static class PrismIocExtensions
    {
        public static IUnityContainer GetInstance(this IContainerProvider containerProvider)
        {
            return ((IContainerExtension<IUnityContainer>)containerProvider).Instance;
        }

        public static IUnityContainer GetInstance(this IContainerRegistry containerRegistry)
        {
            return ((IContainerExtension<IUnityContainer>)containerRegistry).Instance;
        }
    }
}