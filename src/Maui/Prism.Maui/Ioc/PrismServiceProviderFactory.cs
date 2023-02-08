namespace Prism.Ioc;

public class PrismServiceProviderFactory : IServiceProviderFactory<IContainerExtension>
{
    private Action<IContainerExtension> _registerTypes { get; }

    public PrismServiceProviderFactory(Action<IContainerExtension> registerTypes)
    {
        _registerTypes = registerTypes;
    }

    public IContainerExtension CreateBuilder(IServiceCollection services)
    {
        var container = ContainerLocator.Current;
        container.Populate(services);
        _registerTypes(container);

        return container;
    }

    public IServiceProvider CreateServiceProvider(IContainerExtension containerExtension)
    {
        return containerExtension.CreateServiceProvider();
    }
}
