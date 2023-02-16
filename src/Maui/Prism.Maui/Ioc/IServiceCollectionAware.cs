namespace Prism.Ioc;

public interface IServiceCollectionAware
{
    void Populate(IServiceCollection services);
    IServiceProvider CreateServiceProvider();
}
