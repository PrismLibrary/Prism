namespace Prism.Ioc;

public class PrismServiceProviderFactory : IServiceProviderFactory<IContainerExtension>
{
    private readonly IContainerExtension _containerExtension;
    public PrismServiceProviderFactory(IContainerExtension containerExtension)
    {
        _containerExtension = containerExtension;
    }

    private Action<IContainerExtension> _registerTypes { get; }

    public PrismServiceProviderFactory(Action<IContainerExtension> registerTypes)
    {
        _registerTypes = registerTypes;
    }

    public IContainerExtension CreateBuilder(IServiceCollection services)
    {
        _containerExtension.Populate(services);
        _registerTypes(_containerExtension);

        return _containerExtension;
    }

    public IServiceProvider CreateServiceProvider(IContainerExtension containerExtension)
    {
        return containerExtension.CreateServiceProvider();
    }
}
