using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Core.Resolving;
using Autofac.Features.LightweightAdapters;
using Autofac.Features.OpenGenerics;
using Autofac.Features.Scanning;

namespace Prism.Autofac.Forms.Immutable
{
    public class AutofacContainer : IAutofacContainer
    {
        private readonly object _locker = new object();

        private IContainer _container;

        private ContainerBuilder _builder = new ContainerBuilder();

        private IComponentRegistry _runtimeRegistry;

        //TODO: The _registeredTypes field should be eliminated when we can require Autofac 4.4.0 or higher
        private List<Type> _registeredTypes = new List<Type>();

        public bool IsContainerBuilt => _container != null;

        private void CheckBuilder()
        {
            if (IsContainerBuilt)
            {
                throw new InvalidOperationException("It is not possible to perform registration operations after the Container has been built.");
            }
        }

        //TODO: We will be able to eliminate the TrackRegisteredType() method when we can require Autofac 4.4.0 or higher, and do conditional registration.
        private void TrackRegisteredType(Type registeredType)
        {
            if (registeredType == null) return;
            lock (_locker)
            {
                if (!_registeredTypes.Contains(registeredType))
                {
                    _registeredTypes.Add(registeredType);
                }
            }
        }

        //TODO: We will be able to eliminate the IsTypeRegistered() method when we can require Autofac 4.4.0 or higher, and do conditional registration
        [Obsolete("The IsTypeRegistered() method will be removed in the future; if you are using Autofac 4.4.0 (or higher), use conditional registration instead.")]
        public bool IsTypeRegistered(Type registeredType)
        {
            if (registeredType == null) return false;
            lock (_locker)
            {
                return _registeredTypes.Contains(registeredType);
            }
        }

        private void CheckBuildContainer()
        {
            if (IsContainerBuilt) return;
            lock (_locker)
            {
                if (_container == null)
                {
                    _container = _builder.Build();
                    _runtimeRegistry = new RuntimeComponentRegistry(_container);
                }
            }
        }

        public object ResolveComponent(IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            CheckBuildContainer();
            return _container.ResolveComponent(registration, parameters);
        }

        public IComponentRegistry ComponentRegistry
        {
            get
            {
                CheckBuildContainer();
                return _runtimeRegistry;
            }
        }

        #region Registration operations

        /// <summary>Add a component to the container.</summary>
        /// <param name="registration">The component to add.</param>
        public void RegisterComponent(IComponentRegistration registration)
        {
            CheckBuilder();
            //TODO: May need to identify the types being registered, and call TrackRegisteredType() on them
            _builder.RegisterComponent(registration);
        }

        /// <summary>Add a registration source to the container.</summary>
        /// <param name="registrationSource">The registration source to add.</param>
        public void RegisterSource(IRegistrationSource registrationSource)
        {
            CheckBuilder();
            //TODO: May need to identify the types being registered, and call TrackRegisteredType() on them
            _builder.RegisterSource(registrationSource);
        }

        /// <summary>Register an instance as a component.</summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="instance">The instance to register.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        /// <remarks>If no services are explicitly specified for the instance, the
        /// static type <typeparamref name="T" /> will be used as the default service (i.e. *not* <code>instance.GetType()</code>).</remarks>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<T>(T instance) where T : class
        {
            CheckBuilder();
            TrackRegisteredType(typeof(T));
            return _builder.RegisterInstance(instance);
        }

        /// <summary>
        /// Register a component to be created through reflection.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the component implementation.</typeparam>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public IRegistrationBuilder<TImplementer, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<TImplementer>()
        {
            CheckBuilder();
            TrackRegisteredType(typeof(TImplementer));
            return _builder.RegisterType<TImplementer>();
        }

        public IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(Type implementationType)
        {
            CheckBuilder();
            TrackRegisteredType(implementationType);
            return _builder.RegisterType(implementationType);
        }

        /// <summary>Register a delegate as a component.</summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="delegate">The delegate to register.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T>(Func<IComponentContext, T> @delegate)
        {
            CheckBuilder();
            TrackRegisteredType(typeof(T));
            return _builder.Register(@delegate);
        }

        /// <summary>Register a delegate as a component.</summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="delegate">The delegate to register.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T>(Func<IComponentContext, IEnumerable<Parameter>, T> @delegate)
        {
            CheckBuilder();
            TrackRegisteredType(typeof(T));
            return _builder.Register(@delegate);
        }

        /// <summary>
        /// Register an un-parameterised generic type, e.g. Repository&lt;&gt;.
        /// Concrete types will be made as they are requested, e.g. with Resolve&lt;Repository&lt;int&gt;&gt;().
        /// </summary>
        /// <param name="implementer">The open generic implementation type.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> RegisterGeneric(Type implementer)
        {
            CheckBuilder();
            TrackRegisteredType(implementer);
            return _builder.RegisterGeneric(implementer);
        }

        /// <summary>Register the types in an assembly.</summary>
        /// <param name="assemblies">The assemblies from which to register types.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle> RegisterAssemblyTypes(params Assembly[] assemblies)
        {
            CheckBuilder();
            //TODO: May need to identify the types being registered, and call TrackRegisteredType() on them
            return _builder.RegisterAssemblyTypes(assemblies);
        }

        /// <summary>Register the types in a list.</summary>
        /// <param name="types">The types to register.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        public IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle> RegisterTypes(params Type[] types)
        {
            CheckBuilder();
            foreach (Type type in (types ?? new Type[] {}))
            {
                TrackRegisteredType(type);
            }
            return _builder.RegisterTypes(types);
        }

        /// <summary>
        /// Adapt all components implementing service <typeparamref name="TFrom" />
        /// to provide <typeparamref name="TTo" /> using the provided <paramref name="adapter" />
        /// function.
        /// </summary>
        /// <typeparam name="TFrom">Service type to adapt from.</typeparam>
        /// <typeparam name="TTo">Service type to adapt to. Must not be the
        /// same as <typeparamref name="TFrom" />.</typeparam>
        /// <param name="adapter">Function adapting <typeparamref name="TFrom" /> to
        /// service <typeparamref name="TTo" />, given the context and parameters.</param>
        public IRegistrationBuilder<TTo, LightweightAdapterActivatorData, DynamicRegistrationStyle> RegisterAdapter<TFrom, TTo>(Func<IComponentContext, IEnumerable<Parameter>, TFrom, TTo> adapter)
        {
            CheckBuilder();
            TrackRegisteredType(typeof(TTo));
            return _builder.RegisterAdapter(adapter);
        }

        /// <summary>
        /// Adapt all components implementing service <typeparamref name="TFrom" />
        /// to provide <typeparamref name="TTo" /> using the provided <paramref name="adapter" />
        /// function.
        /// </summary>
        /// <typeparam name="TFrom">Service type to adapt from.</typeparam>
        /// <typeparam name="TTo">Service type to adapt to. Must not be the
        /// same as <typeparamref name="TFrom" />.</typeparam>
        /// <param name="adapter">Function adapting <typeparamref name="TFrom" /> to
        /// service <typeparamref name="TTo" />, given the context.</param>
        public IRegistrationBuilder<TTo, LightweightAdapterActivatorData, DynamicRegistrationStyle> RegisterAdapter<TFrom, TTo>(Func<IComponentContext, TFrom, TTo> adapter)
        {
            CheckBuilder();
            TrackRegisteredType(typeof(TTo));
            return _builder.RegisterAdapter(adapter);
        }

        /// <summary>
        /// Adapt all components implementing service <typeparamref name="TFrom" />
        /// to provide <typeparamref name="TTo" /> using the provided <paramref name="adapter" />
        /// function.
        /// </summary>
        /// <typeparam name="TFrom">Service type to adapt from.</typeparam>
        /// <typeparam name="TTo">Service type to adapt to. Must not be the
        /// same as <typeparamref name="TFrom" />.</typeparam>
        /// <param name="adapter">Function adapting <typeparamref name="TFrom" /> to
        /// service <typeparamref name="TTo" />.</param>
        public IRegistrationBuilder<TTo, LightweightAdapterActivatorData, DynamicRegistrationStyle> RegisterAdapter<TFrom, TTo>(Func<TFrom, TTo> adapter)
        {
            CheckBuilder();
            TrackRegisteredType(typeof(TTo));
            return _builder.RegisterAdapter(adapter);
        }

        /// <summary>
        /// Decorate all components implementing open generic service <paramref name="decoratedServiceType" />.
        /// The <paramref name="fromKey" /> and <paramref name="toKey" /> parameters must be different values.
        /// </summary>
        /// <param name="decoratedServiceType">Service type being decorated. Must be an open generic type.</param>
        /// <param name="fromKey">Service key or name associated with the components being decorated.</param>
        /// <param name="toKey">Service key or name given to the decorated components.</param>
        /// <param name="decoratorType">The type of the decorator. Must be an open generic type, and accept a parameter
        /// of type <paramref name="decoratedServiceType" />, which will be set to the instance being decorated.</param>
        public IRegistrationBuilder<object, OpenGenericDecoratorActivatorData, DynamicRegistrationStyle> RegisterGenericDecorator(Type decoratorType, Type decoratedServiceType, object fromKey, object toKey = null)
        {
            CheckBuilder();
            TrackRegisteredType(decoratedServiceType);
            return _builder.RegisterGenericDecorator(decoratorType, decoratedServiceType, fromKey, toKey);
        }

        /// <summary>
        /// Decorate all components implementing service <typeparamref name="TService" />
        /// using the provided <paramref name="decorator" /> function.
        /// The <paramref name="fromKey" /> and <paramref name="toKey" /> parameters must be different values.
        /// </summary>
        /// <typeparam name="TService">Service type being decorated.</typeparam>
        /// <param name="decorator">Function decorating a component instance that provides
        /// <typeparamref name="TService" />, given the context and parameters.</param>
        /// <param name="fromKey">Service key or name associated with the components being decorated.</param>
        /// <param name="toKey">Service key or name given to the decorated components.</param>
        public IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle> RegisterDecorator<TService>(Func<IComponentContext, IEnumerable<Parameter>, TService, TService> decorator, object fromKey, object toKey = null)
        {
            CheckBuilder();
            TrackRegisteredType(typeof(TService));
            return _builder.RegisterDecorator(decorator, fromKey, toKey);
        }

        /// <summary>
        /// Decorate all components implementing service <typeparamref name="TService" />
        /// using the provided <paramref name="decorator" /> function.
        /// The <paramref name="fromKey" /> and <paramref name="toKey" /> parameters must be different values.
        /// </summary>
        /// <typeparam name="TService">Service type being decorated.</typeparam>
        /// <param name="decorator">Function decorating a component instance that provides
        /// <typeparamref name="TService" />, given the context.</param>
        /// <param name="fromKey">Service key or name associated with the components being decorated.</param>
        /// <param name="toKey">Service key or name given to the decorated components.</param>
        public IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle> RegisterDecorator<TService>(Func<IComponentContext, TService, TService> decorator, object fromKey, object toKey = null)
        {
            CheckBuilder();
            TrackRegisteredType(typeof(TService));
            return _builder.RegisterDecorator(decorator, fromKey, toKey);
        }

        /// <summary>
        /// Decorate all components implementing service <typeparamref name="TService" />
        /// using the provided <paramref name="decorator" /> function.
        /// The <paramref name="fromKey" /> and <paramref name="toKey" /> parameters must be different values.
        /// </summary>
        /// <typeparam name="TService">Service type being decorated.</typeparam>
        /// <param name="decorator">Function decorating a component instance that provides
        /// <typeparamref name="TService" />.</param>
        /// <param name="fromKey">Service key or name associated with the components being decorated.</param>
        /// <param name="toKey">Service key or name given to the decorated components.</param>
        public IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle> RegisterDecorator<TService>(Func<TService, TService> decorator, object fromKey, object toKey = null)
        {
            CheckBuilder();
            TrackRegisteredType(typeof(TService));
            return _builder.RegisterDecorator(decorator, fromKey, toKey);
        }

        #endregion

        public void Dispose()
        {
            _runtimeRegistry?.Dispose();
            _runtimeRegistry = null;
            _builder = null;
            _container?.Dispose();
            _container = null;
            _registeredTypes?.Clear();
            _registeredTypes = null;
        }

        public ILifetimeScope BeginLifetimeScope()
        {
            return _container?.BeginLifetimeScope();
        }

        public ILifetimeScope BeginLifetimeScope(object tag)
        {
            return _container?.BeginLifetimeScope(tag);
        }

        public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return _container?.BeginLifetimeScope(configurationAction);
        }

        public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
        {
            return _container?.BeginLifetimeScope(tag, configurationAction);
        }

        public IDisposer Disposer => _container?.Disposer;

        public object Tag => _container?.Tag;

        //These events appear to not be used by Prism.Autofac.Forms
        public event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning;
        public event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding;
        public event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning;
    }
}
