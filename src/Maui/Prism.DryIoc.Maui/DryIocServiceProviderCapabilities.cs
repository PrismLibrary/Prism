using DryIoc;
using Prism.Ioc;
using IContainer = DryIoc.IContainer;

namespace Prism.DryIoc;

internal sealed class DryIocServiceProviderCapabilities : IServiceProviderIsService, ISupportRequiredService
{
    private readonly IContainer _container;
    /// <summary>Statefully wraps the passed <paramref name="container"/></summary>
    public DryIocServiceProviderCapabilities(IContainer container) => _container = container;

    /// <inheritdoc />
    public bool IsService(Type serviceType)
    {
        // I am not fully comprehend but MS.DI considers asking for the open-generic type even if it is registered to return `false`
        // Probably mixing here the fact that open type cannot be instantiated without providing the concrete type argument.
        // But I think it is conflating two things and making the reasoning harder.
        if (serviceType.IsGenericTypeDefinition)
            return false;

        if (serviceType == typeof(IServiceProviderIsService) ||
            serviceType == typeof(ISupportRequiredService) ||
            serviceType == typeof(IServiceScopeFactory))
            return true;

        if (_container.IsRegistered(serviceType))
            return true;

        if (serviceType.IsGenericType &&
            _container.IsRegistered(serviceType.GetGenericTypeDefinition()))
            return true;

        return _container.IsRegistered(serviceType, factoryType: FactoryType.Wrapper);
    }

    /// <inheritdoc />
    public object GetRequiredService(Type serviceType) => _container.Resolve(serviceType);
}
