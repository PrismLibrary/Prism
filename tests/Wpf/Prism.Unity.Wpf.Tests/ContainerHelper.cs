using System;
using Prism.Ioc;
using Prism.Unity;
using Unity;

namespace Prism.Container.Wpf.Tests
{
    public static class ContainerHelper
    {
        public static IUnityContainer CreateContainer() =>
            new UnityContainer();

        public static IContainerExtension CreateContainerExtension() =>
            new UnityContainerExtension(CreateContainer());

        public static IUnityContainer GetBaseContainer(this IContainerExtension container) =>
            ((IContainerProvider)container).GetContainer();

        public static IUnityContainer GetBaseContainer(this IContainerProvider container) =>
            container.GetContainer();

        public static Type ContainerExtensionType => typeof(UnityContainerExtension);

        public static Type BaseContainerType => typeof(UnityContainer);

        public static Type BaseContainerInterfaceType = typeof(IUnityContainer);

        public static Type RegisteredFrameworkException = typeof(ResolutionFailedException);
    }
}
