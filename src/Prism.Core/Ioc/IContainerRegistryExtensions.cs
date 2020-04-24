using System;

namespace Prism.Ioc
{
    /// <summary>
    /// Provides Generic Type extensions for the <see cref="IContainerRegistry" />
    /// </summary>
    public static class IContainerRegistryExtensions
    {
        /// <summary>
        /// Registers an instance of a given <see cref="Type"/>
        /// </summary>
        /// <typeparam name="TInterface">The service <see cref="Type"/> that is being registered</typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterInstance<TInterface>(this IContainerRegistry containerRegistry, TInterface instance)
        {
            return containerRegistry.RegisterInstance(typeof(TInterface), instance);
        }

        /// <summary>
        /// Registers an instance of a given <see cref="Type"/> with the specified name or key
        /// </summary>
        /// <typeparam name="TInterface">The service <see cref="Type"/> that is being registered</typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterInstance<TInterface>(this IContainerRegistry containerRegistry, TInterface instance, string name)
        {
            return containerRegistry.RegisterInstance(typeof(TInterface), instance, name);
        }

        /// <summary>
        /// Registers a Singleton with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="type">The concrete <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton(this IContainerRegistry containerRegistry, Type type)
        {
            return containerRegistry.RegisterSingleton(type, type);
        }

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TFrom">The service <see cref="Type" /></typeparam>
        /// <typeparam name="TTo">The implementation <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton<TFrom, TTo>(this IContainerRegistry containerRegistry) where TTo : TFrom
        {
            return containerRegistry.RegisterSingleton(typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TFrom">The service <see cref="Type" /></typeparam>
        /// <typeparam name="TTo">The implementation <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton<TFrom, TTo>(this IContainerRegistry containerRegistry, string name) where TTo : TFrom
        {
            return containerRegistry.RegisterSingleton(typeof(TFrom), typeof(TTo), name);
        }

        /// <summary>
        /// Registers a Singleton with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <typeparam name="T">The concrete <see cref="Type" /></typeparam>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton<T>(this IContainerRegistry containerRegistry)
        {
            return containerRegistry.RegisterSingleton(typeof(T));
        }

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton<T>(this IContainerRegistry containerRegistry, Func<object> factoryMethod)
        {
            return containerRegistry.RegisterSingleton(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton<T>(this IContainerRegistry containerRegistry, Func<IContainerProvider, object> factoryMethod)
        {
            return containerRegistry.RegisterSingleton(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Registers a Singleton Service which implements service interfaces
        /// </summary>
        /// <typeparam name="T">The implementation <see cref="Type" />.</typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        public static IContainerRegistry RegisterManySingleton<T>(this IContainerRegistry containerRegistry, params Type[] serviceTypes)
        {
            return containerRegistry.RegisterManySingleton(typeof(T), serviceTypes);
        }

        /// <summary>
        /// Registers a Transient with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="type">The concrete <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register(this IContainerRegistry containerRegistry, Type type)
        {
            return containerRegistry.Register(type, type);
        }

        /// <summary>
        /// Registers a Transient with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <typeparam name="T">The concrete <see cref="Type" /></typeparam>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<T>(this IContainerRegistry containerRegistry)
        {
            return containerRegistry.Register(typeof(T));
        }

        /// <summary>
        /// Registers a Transient with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="type">The concrete <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register(this IContainerRegistry containerRegistry, Type type, string name)
        {
            return containerRegistry.Register(type, type, name);
        }

        /// <summary>
        /// Registers a Singleton with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <typeparam name="T">The concrete <see cref="Type" /></typeparam>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<T>(this IContainerRegistry containerRegistry, string name)
        {
            return containerRegistry.Register(typeof(T), name);
        }

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TFrom">The service <see cref="Type" /></typeparam>
        /// <typeparam name="TTo">The implementation <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<TFrom, TTo>(this IContainerRegistry containerRegistry) where TTo : TFrom
        {
            return containerRegistry.Register(typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TFrom">The service <see cref="Type" /></typeparam>
        /// <typeparam name="TTo">The implementation <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<TFrom, TTo>(this IContainerRegistry containerRegistry, string name) where TTo : TFrom
        {
            return containerRegistry.Register(typeof(TFrom), typeof(TTo), name);
        }

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<T>(this IContainerRegistry containerRegistry, Func<object> factoryMethod)
        {
            return containerRegistry.Register(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<T>(this IContainerRegistry containerRegistry, Func<IContainerProvider, object> factoryMethod)
        {
            return containerRegistry.Register(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Registers a Transient Service which implements service interfaces
        /// </summary>
        /// <typeparam name="T">The implementing <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        public static IContainerRegistry RegisterMany<T>(this IContainerRegistry containerRegistry, params Type[] serviceTypes)
        {
            return containerRegistry.RegisterMany(typeof(T), serviceTypes);
        }

        /// <summary>
        /// Registers a scoped service.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="type">The concrete <see cref="Type" />.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterScoped(this IContainerRegistry containerRegistry, Type type)
        {
            return containerRegistry.RegisterScoped(type, type);
        }

        /// <summary>
        /// Registers a scoped service.
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterScoped<T>(this IContainerRegistry containerRegistry)
        {
            return containerRegistry.RegisterScoped(typeof(T));
        }

        /// <summary>
        /// Registers a scoped service
        /// </summary>
        /// <typeparam name="TFrom">The service <see cref="Type" /></typeparam>
        /// <typeparam name="TTo">The implementation <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterScoped<TFrom, TTo>(this IContainerRegistry containerRegistry)
            where TTo : TFrom
        {
            return containerRegistry.RegisterScoped(typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterScoped<T>(this IContainerRegistry containerRegistry, Func<object> factoryMethod)
        {
            return containerRegistry.RegisterScoped(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterScoped<T>(this IContainerRegistry containerRegistry, Func<IContainerProvider, object> factoryMethod)
        {
            return containerRegistry.RegisterScoped(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Determines if a given service is registered
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public static bool IsRegistered<T>(this IContainerRegistry containerRegistry)
        {
            return containerRegistry.IsRegistered(typeof(T));
        }

        /// <summary>
        /// Determines if a given service is registered with the specified name
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="name">The service name or key used</param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public static bool IsRegistered<T>(this IContainerRegistry containerRegistry, string name)
        {
            return containerRegistry.IsRegistered(typeof(T), name);
        }
    }
}
