using System;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;

namespace Prism.Container.Wpf.Tests
{
    public static class ContainerHelper
    {
        private static Rules CreateContainerRules() => Rules.Default.WithAutoConcreteTypeResolution()
                                                                    .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                                                                    .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace);

        public static IContainer CreateContainer() =>
            new global::DryIoc.Container(CreateContainerRules());

        public static IContainerExtension CreateContainerExtension() =>
        new DryIocContainerExtension(CreateContainer());

        public static IContainer GetBaseContainer(this IContainerExtension container) =>
            ((IContainerProvider)container).GetContainer();

        public static IContainer GetBaseContainer(this IContainerProvider container) =>
            container.GetContainer();

        public static Type ContainerExtensionType => typeof(DryIocContainerExtension);

        public static Type BaseContainerType => typeof(global::DryIoc.Container);

        public static Type BaseContainerInterfaceType = typeof(IContainer);

        public static Type RegisteredFrameworkException = typeof(ContainerException);
    }
}
