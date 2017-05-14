using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.LightweightAdapters;
using Autofac.Features.OpenGenerics;
using Autofac.Features.Scanning;

// ReSharper disable once CheckNamespace
namespace Prism.Autofac.Forms
{
    /// <summary>
    /// An abstraction of an Autofac ContainerBuilder plus Container - wraps an Autofac IContainer
    /// instance that can only be built once and cannot be updated (i.e. is immutable).
    /// </summary>
    public interface IAutofacContainer : IContainer
    {
        /// <summary>
        /// Identifies if the wrapped Autofac IContainer instance has been built or not.
        /// (Type/page registrations can no longer be made after it has been built.
        /// </summary>
        bool IsContainerBuilt { get; }

        //TODO: We will be able to eliminate the IsTypeRegistered() method when we can require Autofac 4.4.0 or higher, and do conditional registration
        /// <summary>
        /// Identifies if a particular Type has already been registered for the wrapped IContainer instance (to be built)
        /// </summary>
        /// <param name="registeredType">The Type to check</param>
        /// <returns>True if the Type is already registered, False if it is not.</returns>
        [Obsolete("The IsTypeRegistered() method will be removed in the future; if you are using Autofac 4.4.0 (or higher), use conditional registration instead.")]
        bool IsTypeRegistered(Type registeredType);

        #region Registration operations

        /// <summary>Add a component to the container.</summary>
        /// <param name="registration">The component to add.</param>
        void RegisterComponent(IComponentRegistration registration);

        /// <summary>Add a registration source to the container.</summary>
        /// <param name="registrationSource">The registration source to add.</param>
        void RegisterSource(IRegistrationSource registrationSource);

        /// <summary>Register an instance as a component.</summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="instance">The instance to register.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        /// <remarks>If no services are explicitly specified for the instance, the
        /// static type <typeparamref name="T" /> will be used as the default service (i.e. *not* <code>instance.GetType()</code>).</remarks>
        IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<T>(T instance)
            where T : class;

        /// <summary>
        /// Register a component to be created through reflection.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the component implementation.</typeparam>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        IRegistrationBuilder<TImplementer, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterType<TImplementer>();

        IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(
            Type implementationType);

        /// <summary>Register a delegate as a component.</summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="delegate">The delegate to register.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T>(
            Func<IComponentContext, T> @delegate);

        /// <summary>Register a delegate as a component.</summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="delegate">The delegate to register.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T>(
            Func<IComponentContext, IEnumerable<Parameter>, T> @delegate);

        /// <summary>
        /// Register an un-parameterised generic type, e.g. Repository&lt;&gt;.
        /// Concrete types will be made as they are requested, e.g. with Resolve&lt;Repository&lt;int&gt;&gt;().
        /// </summary>
        /// <param name="implementer">The open generic implementation type.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> RegisterGeneric(
            Type implementer);

        /// <summary>Register the types in an assembly.</summary>
        /// <param name="assemblies">The assemblies from which to register types.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle> RegisterAssemblyTypes(
            params Assembly[] assemblies);

        /// <summary>Register the types in a list.</summary>
        /// <param name="types">The types to register.</param>
        /// <returns>Registration builder allowing the registration to be configured.</returns>
        IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
            RegisterTypes(params Type[] types);

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
        IRegistrationBuilder<TTo, LightweightAdapterActivatorData, DynamicRegistrationStyle>
            RegisterAdapter<TFrom, TTo>(Func<IComponentContext, IEnumerable<Parameter>, TFrom, TTo> adapter);

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
        IRegistrationBuilder<TTo, LightweightAdapterActivatorData, DynamicRegistrationStyle>
            RegisterAdapter<TFrom, TTo>(Func<IComponentContext, TFrom, TTo> adapter);

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
        IRegistrationBuilder<TTo, LightweightAdapterActivatorData, DynamicRegistrationStyle>
            RegisterAdapter<TFrom, TTo>(Func<TFrom, TTo> adapter);

        /// <summary>
        /// Decorate all components implementing open generic service <paramref name="decoratedServiceType" />.
        /// The <paramref name="fromKey" /> and <paramref name="toKey" /> parameters must be different values.
        /// </summary>
        /// <param name="decoratedServiceType">Service type being decorated. Must be an open generic type.</param>
        /// <param name="fromKey">Service key or name associated with the components being decorated.</param>
        /// <param name="toKey">Service key or name given to the decorated components.</param>
        /// <param name="decoratorType">The type of the decorator. Must be an open generic type, and accept a parameter
        /// of type <paramref name="decoratedServiceType" />, which will be set to the instance being decorated.</param>
        IRegistrationBuilder<object, OpenGenericDecoratorActivatorData, DynamicRegistrationStyle>
            RegisterGenericDecorator(Type decoratorType, Type decoratedServiceType, object fromKey,
                object toKey = null);

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
        IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle>
            RegisterDecorator<TService>(Func<IComponentContext, IEnumerable<Parameter>, TService, TService> decorator,
                object fromKey, object toKey = null);

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
        IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle>
            RegisterDecorator<TService>(Func<IComponentContext, TService, TService> decorator, object fromKey,
                object toKey = null);

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
        IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle>
            RegisterDecorator<TService>(Func<TService, TService> decorator, object fromKey, object toKey = null);

        #endregion
    }
}
