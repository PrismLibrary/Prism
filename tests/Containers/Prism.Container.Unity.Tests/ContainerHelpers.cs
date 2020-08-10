using Unity;

namespace Prism.Ioc.Tests
{
    public static class ContainerHelpers
    {
        public static IUnityContainer GetContainer(this IContainerExtension container) =>
            Prism.Unity.PrismIocExtensions.GetContainer((IContainerProvider)container);
    }
}
