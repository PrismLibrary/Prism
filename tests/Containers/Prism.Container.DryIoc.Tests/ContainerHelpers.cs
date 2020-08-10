using DryIoc;

namespace Prism.Ioc.Tests
{
    public static class ContainerHelpers
    {
        public static IContainer GetContainer(this IContainerExtension container) =>
            Prism.DryIoc.PrismIocExtensions.GetContainer((IContainerProvider)container);
    }
}
