using DryIoc;
using Prism.Container.DryIoc;
using Prism.Ioc;

namespace Prism.Container.Uno.Tests;

public static class ContainerHelper
{
    private static Rules CreateContainerRules() => Rules.Default.WithAutoConcreteTypeResolution()
                                                                .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                                                                .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace);

    public static IContainer CreateContainer() => new global::DryIoc.Container(CreateContainerRules());

    public static IContainerExtension CreateContainerExtension() => new DryIocContainerExtension(CreateContainer());

    public static Type RegisteredFrameworkException => typeof(ContainerException);
}
