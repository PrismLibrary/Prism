using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Unity;
using Prism.Unity.Ioc;
using Unity;

namespace Prism.Container.Wpf.Tests
{
    public static class ContainerHelper
    {
        public static IUnityContainer CreateContainer() =>
            new UnityContainer();

        public static IContainerExtension CreateContainerExtension() =>
            new UnityContainerExtension(CreateContainer());

        public const string CollectionName = "ContainerExtension";

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
