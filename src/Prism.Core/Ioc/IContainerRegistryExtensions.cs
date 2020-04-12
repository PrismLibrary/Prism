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
