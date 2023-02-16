using DryIoc;
using Prism.Ioc;
using IContainer = DryIoc.IContainer;

namespace Prism.DryIoc;

partial class DryIocContainerExtension : IServiceCollectionAware
{
    public IServiceProvider CreateServiceProvider()
    {
        var capabilities = new DryIocServiceProviderCapabilities(Instance);
        var singletons = Instance.SingletonScope;
        singletons.Use<IServiceProviderIsService>(capabilities);
        singletons.Use<ISupportRequiredService>(capabilities);
        singletons.UseFactory<IServiceScopeFactory>(r => new DryIocServiceScopeFactory(r));

        return Instance;
    }

    public void Populate(IServiceCollection services)
    {
        foreach (var descriptor in services)
            RegisterDescriptor(Instance, descriptor);

        var errors = Instance.Validate();
    }

    static IReuse ToReuse(ServiceLifetime lifetime) =>
        lifetime == ServiceLifetime.Singleton ? Reuse.Singleton :
        lifetime == ServiceLifetime.Scoped ? Reuse.ScopedOrSingleton : // see, that we have Reuse.ScopedOrSingleton here instead of Reuse.Scoped
        Reuse.Transient;

    static void RegisterDescriptor(IContainer container, ServiceDescriptor descriptor)
    {
        var serviceType = descriptor.ServiceType;
        var implType = descriptor.ImplementationType;
        if (implType != null)
        {
            container.Register(ReflectionFactory.Of(implType, ToReuse(descriptor.Lifetime)), serviceType,
                null, null, isStaticallyChecked: implType == serviceType);
        }
        else if (descriptor.ImplementationFactory != null)
        {
            container.Register(DelegateFactory.Of(descriptor.ImplementationFactory.ToFactoryDelegate, ToReuse(descriptor.Lifetime)), serviceType,
                null, null, isStaticallyChecked: true);
        }
        else
        {
            var instance = descriptor.ImplementationInstance;
            container.Register(InstanceFactory.Of(instance), serviceType,
                null, null, isStaticallyChecked: true);
            container.TrackDisposable(instance);
        }
    }
}
