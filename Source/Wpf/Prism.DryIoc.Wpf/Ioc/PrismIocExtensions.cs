using DryIoc;
using Prism.Ioc;

namespace Prism.DryIoc
{
    public static class PrismIocExtensions
    {
        public static IContainer GetInstance(this IContainerProvider containerProvider)
        {
            return ((IContainerExtension<IContainer>)containerProvider).Instance;
        }

        public static IContainer GetInstance(this IContainerRegistry containerRegistry)
        {
            return ((IContainerExtension<IContainer>)containerRegistry).Instance;
        }
    }
}
