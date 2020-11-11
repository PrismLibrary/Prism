using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prism.Ioc;
using Prism.Ioc.Internals;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;

namespace Prism.Unity
{
    /// <summary>
    /// The Unity implementation of the <see cref="IContainerExtension" />
    /// </summary>
#if ContainerExtensions
    internal partial
#else
    public
#endif
    class UnityContainerExtension : IContainerExtension<IUnityContainer>, IContainerInfo
    {
        private UnityScopedProvider _currentScope;

        /// <summary>
        /// The instance of the wrapped container
        /// </summary>
        public IUnityContainer Instance { get; }

#if !ContainerExtensions
        /// <summary>
        /// Constructs a default <see cref="UnityContainerExtension" />
        /// </summary>
        public UnityContainerExtension()
            : this(new UnityContainer())
        {
        }

        /// <summary>
        /// Constructs a <see cref="UnityContainerExtension" /> with the specified <see cref="IUnityContainer" />
        /// </summary>
        /// <param name="container"></param>
        public UnityContainerExtension(IUnityContainer container)
        {
            Instance = container;
            string currentContainer = "CurrentContainer";
            Instance.RegisterInstance(currentContainer, this);
            Instance.RegisterFactory(typeof(IContainerExtension), c => c.Resolve<UnityContainerExtension>(currentContainer));
            Instance.RegisterFactory(typeof(IContainerProvider), c => c.Resolve<UnityContainerExtension>(currentContainer));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ResolutionFailedException));
        }
#endif

        /// <summary>
        /// Gets the current <see cref="IScopedProvider"/>
        /// </summary>
        public IScopedProvider CurrentScope => _currentScope;

        /// <summary>
        /// Used to perform any final steps for configuring the extension that may be required by the container.
        /// </summary>
        public void FinalizeExtension() { }

        /// <summary>
        /// Registers an instance of a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/> that is being registered</param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            Instance.RegisterInstance(type, instance);
            return this;
        }

        /// <summary>
        /// Registers an instance of a given <see cref="Type"/> with the specified name or key
        /// </summary>
        /// <param name="type">The service <see cref="Type"/> that is being registered</param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            Instance.RegisterInstance(type, name, instance);
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            Instance.RegisterSingleton(from, to);
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            Instance.RegisterSingleton(from, to, name);
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod)
        {
            Instance.RegisterFactory(type, _ => factoryMethod(), new ContainerControlledLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.RegisterFactory(type, c => factoryMethod(c.Resolve<IContainerProvider>()), new ContainerControlledLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers a Singleton Service which implements service interfaces
        /// </summary>
        /// <param name="type">The implementation <see cref="Type" />.</param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes)
        {
            Instance.RegisterSingleton(type);
            return RegisterManyInternal(type, serviceTypes);
        }

        private IContainerRegistry RegisterManyInternal(Type implementingType, Type[] serviceTypes)
        {
            if (serviceTypes is null || serviceTypes.Length == 0)
            {
                serviceTypes = implementingType.GetInterfaces().Where(x => x != typeof(IDisposable)).ToArray();
            }

            foreach (var service in serviceTypes)
            {
                Instance.RegisterFactory(service, c => c.Resolve(implementingType));
            }

            return this;
        }

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type from, Type to)
        {
            Instance.RegisterType(from, to);
            return this;
        }

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type from, Type to, string name)
        {
            Instance.RegisterType(from, to, name);
            return this;
        }

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type type, Func<object> factoryMethod)
        {
            Instance.RegisterFactory(type, _ => factoryMethod());
            return this;
        }

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.RegisterFactory(type, c => factoryMethod(c.Resolve<IContainerProvider>()));
            return this;
        }

        /// <summary>
        /// Registers a Transient Service which implements service interfaces
        /// </summary>
        /// <param name="type">The implementing <see cref="Type" />.</param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes)
        {
            Instance.RegisterType(type);
            return RegisterManyInternal(type, serviceTypes);
        }

        /// <summary>
        /// Registers a scoped service
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterScoped(Type from, Type to)
        {
            Instance.RegisterType(from, to, new HierarchicalLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod)
        {
            Instance.RegisterFactory(type, c => factoryMethod(), new HierarchicalLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type"/>.</param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.RegisterFactory(type, c => factoryMethod(c.Resolve<IContainerProvider>()), new HierarchicalLifetimeManager());
            return this;
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type) =>
            Resolve(type, Array.Empty<(Type, object)>());

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type, string name) =>
            Resolve(type, name, Array.Empty<(Type, object)>());

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            try
            {
                var c = _currentScope?.Container ?? Instance;
                var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();

                if (typeof(IEnumerable).IsAssignableFrom(type) && type.GetGenericArguments().Length > 0)
                {
                    type = type.GetGenericArguments()[0];
                    return c.ResolveAll(type, overrides);
                }

                return c.Resolve(type, overrides);
            }
            catch (Exception ex)
            {
                throw new ContainerResolutionException(type, ex);
            }
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            try
            {
                var c = _currentScope?.Container ?? Instance;

                // Unity will simply return a new object() for unregistered Views
                if (!c.IsRegistered(type, name))
                    throw new KeyNotFoundException($"No registered type {type.Name} with the key {name}.");

                var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();
                return c.Resolve(type, name, overrides);
            }
            catch (Exception ex)
            {
                throw new ContainerResolutionException(type, name, ex);
            }
        }

        /// <summary>
        /// Determines if a given service is registered
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public bool IsRegistered(Type type)
        {
            return Instance.IsRegistered(type);
        }

        /// <summary>
        /// Determines if a given service is registered with the specified name
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="name">The service name or key used</param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public bool IsRegistered(Type type, string name)
        {
            return Instance.IsRegistered(type, name);
        }

        Type IContainerInfo.GetRegistrationType(string key)
        {
            //First try friendly name registration. If not found, try type registration
            var matchingRegistration = Instance.Registrations.Where(r => key.Equals(r.Name, StringComparison.Ordinal)).FirstOrDefault();
            if (matchingRegistration == null)
            {
                matchingRegistration = Instance.Registrations.Where(r => key.Equals(r.RegisteredType.Name, StringComparison.Ordinal)).FirstOrDefault();
            }

            return matchingRegistration?.MappedToType;
        }

        Type IContainerInfo.GetRegistrationType(Type serviceType)
        {
            var matchingRegistration = Instance.Registrations.Where(x => x.RegisteredType == serviceType).FirstOrDefault();
            return matchingRegistration?.MappedToType;
        }

        /// <summary>
        /// Creates a new Scope
        /// </summary>
        public virtual IScopedProvider CreateScope() =>
            CreateScopeInternal();

        /// <summary>
        /// Creates a new Scope and provides the updated ServiceProvider
        /// </summary>
        /// <returns>A child <see cref="IUnityContainer" />.</returns>
        /// <remarks>
        /// This should be called by custom implementations that Implement IServiceScopeFactory
        /// </remarks>
        protected IScopedProvider CreateScopeInternal()
        {
            var child = Instance.CreateChildContainer();
            _currentScope = new UnityScopedProvider(child);
            return _currentScope;
        }

        private class UnityScopedProvider : IScopedProvider
        {
            public UnityScopedProvider(IUnityContainer container)
            {
                Container = container;
            }

            public IUnityContainer Container { get; private set; }
            public bool IsAttached { get; set; }
            public IScopedProvider CurrentScope => this;

            public IScopedProvider CreateScope() => this;

            public void Dispose()
            {
                Container.Dispose();
                Container = null;
            }

            public object Resolve(Type type) =>
                Resolve(type, Array.Empty<(Type, object)>());

            public object Resolve(Type type, string name) =>
                Resolve(type, name, Array.Empty<(Type, object)>());

            public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();
                    return Container.Resolve(type, overrides);
                }
                catch (Exception ex)
                {
                    throw new ContainerResolutionException(type, ex);
                }
            }

            public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    // Unity will simply return a new object() for unregistered Views
                    if (!Container.IsRegistered(type, name))
                        throw new KeyNotFoundException($"No registered type {type.Name} with the key {name}.");

                    var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();
                    return Container.Resolve(type, name, overrides);
                }
                catch (Exception ex)
                {
                    throw new ContainerResolutionException(type, name, ex);
                }
            }
        }
    }
}
