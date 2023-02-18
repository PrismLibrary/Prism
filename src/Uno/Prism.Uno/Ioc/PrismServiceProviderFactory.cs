namespace Prism.Ioc;

public class PrismServiceProviderFactory : IServiceProviderFactory<IContainerExtension>
{
    private readonly IContainerExtension _containerExtension;
    public PrismServiceProviderFactory(IContainerExtension containerExtension)
    {
        _containerExtension = containerExtension;
    }

    public IContainerExtension CreateBuilder(IServiceCollection services)
    {
        _containerExtension.Populate(services);
        return _containerExtension;
    }

    public IServiceProvider CreateServiceProvider(IContainerExtension containerExtension)
    {
        return containerExtension.CreateServiceProvider();
    }
}
