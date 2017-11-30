using Grace.DependencyInjection;
using Prism.Ioc;

namespace Prism.Grace.Ioc
{
    public static class PrismIocExtensions
    {
        public static DependencyInjectionContainer GetContainer(this IContainerProvider containerProvider)
        {
            return ((IContainerExtension<DependencyInjectionContainer>)containerProvider).Instance;
        }

        public static DependencyInjectionContainer GetContainer(this IContainerRegistry containerRegistry)
        {
            return ((IContainerExtension<DependencyInjectionContainer>)containerRegistry).Instance;
        }
    }
}