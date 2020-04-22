using System;
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
    public class UnityContainerExtension : IContainerExtension<IUnityContainer>, IContainerInfo
    {
        private ServiceScope _currentScope;

        /// <summary>
        /// The instance of the wrapped container
        /// </summary>
        public IUnityContainer Instance { get; }

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
        }

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
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type)
        {
            return Instance.Resolve(type);
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type, string name)
        {
            return Instance.Resolve(type, name);
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();
            return Instance.Resolve(type, overrides);
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
            var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();
            return Instance.Resolve(type, name, overrides);
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

        /// <summary>
        /// Creates a new Scope
        /// </summary>
        public virtual void CreateScope() =>
            CreateScopeInternal();

        /// <summary>
        /// Creates a new Scope and provides the updated ServiceProvider
        /// </summary>
        /// <returns>The Scoped <see cref="IServiceProvider" />.</returns>
        /// <remarks>
        /// This should be called by custom implementations that Implement IServiceScopeFactory
        /// </remarks>
        protected IServiceProvider CreateScopeInternal()
        {
            if (_currentScope != null)
            {
                _currentScope.Dispose();
                _currentScope = null;
                GC.Collect();
            }

            _currentScope = new ServiceScope(Instance.CreateChildContainer());
            return _currentScope;
        }

        private class ServiceScope : IServiceProvider//, IServiceScope
        {
            public ServiceScope(IUnityContainer container)
            {
                Container = container;
            }

            public IUnityContainer Container { get; private set; }

            public IServiceProvider ServiceProvider => this;

            public object GetService(Type serviceType)
            {
                if (!Container.IsRegistered(serviceType))
                    return null;

                return Container.Resolve(serviceType);
            }

            public void Dispose()
            {
                if (Container != null)
                {
                    Container.Dispose();
                    Container = null;
                }

                GC.Collect();
            }
        }
    }
}
