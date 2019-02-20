using System;
using Prism.Ioc;
using Unity;
using Unity.Resolution;

namespace Prism.Unity
{
    public static class PrismIocExtensions
    {
        public static IUnityContainer GetContainer(this IContainerProvider containerProvider)
        {
            return ((IContainerExtension<IUnityContainer>)containerProvider).Instance;
        }

        public static IUnityContainer GetContainer(this IContainerRegistry containerRegistry)
        {
            return ((IContainerExtension<IUnityContainer>)containerRegistry).Instance;
        }
    }
}
