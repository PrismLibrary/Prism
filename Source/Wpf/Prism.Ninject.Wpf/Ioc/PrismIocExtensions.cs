using Ninject;
using Prism.Ioc;

namespace Prism.Ninject
{
    public static class PrismIocExtensions
    {
        public static IKernel GetInstance(this IContainerProvider containerProvider)
        {
            return ((IContainerExtension<IKernel>)containerProvider).Instance;
        }

        public static IKernel GetInstance(this IContainerRegistry containerRegistry)
        {
            return ((IContainerExtension<IKernel>)containerRegistry).Instance;
        }
    }
}
