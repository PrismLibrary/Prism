using System;

namespace Prism.Ioc
{
    /// <summary>
    /// Provides Generic Type extensions for the <see cref="IContainerProvider" />
    /// </summary>
    public static class IContainerProviderExtensions
    {
        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type"/></typeparam>
        /// <param name="provider">The current <see cref="IContainerProvider"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public static T Resolve<T>(this IContainerProvider provider)
        {
            return (T)provider.Resolve(typeof(T));
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type"/></typeparam>
        /// <param name="provider">The current <see cref="IContainerProvider"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public static T Resolve<T>(this IContainerProvider provider, params (Type Type, object Instance)[] parameters)
        {
            return (T)provider.Resolve(typeof(T), parameters);
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type"/></typeparam>
        /// <param name="provider">The current <see cref="IContainerProvider"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public static T Resolve<T>(this IContainerProvider provider, string name, params (Type Type, object Instance)[] parameters)
        {
            return (T)provider.Resolve(typeof(T), name, parameters);
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type"/></typeparam>
        /// <param name="provider">The current <see cref="IContainerProvider"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public static T Resolve<T>(this IContainerProvider provider, string name)
        {
            return (T)provider.Resolve(typeof(T), name);
        }

        /// <summary>
        /// Determines if a given service is registered
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerProvider">The instance of the <see cref="IContainerProvider" /></param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public static bool IsRegistered<T>(this IContainerProvider containerProvider)
        {
            return containerProvider.IsRegistered(typeof(T));
        }

        internal static bool IsRegistered(this IContainerProvider containerProvider, Type type)
        {
            if (containerProvider is IContainerRegistry containerRegistry)
                return containerRegistry.IsRegistered(type);
            return false;
        }

        /// <summary>
        /// Determines if a given service is registered with the specified name
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerProvider">The instance of the <see cref="IContainerProvider" /></param>
        /// <param name="name">The service name or key used</param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public static bool IsRegistered<T>(this IContainerProvider containerProvider, string name)
        {
            return containerProvider.IsRegistered(typeof(T), name);
        }

        internal static bool IsRegistered(this IContainerProvider containerProvider, Type type, string name)
        {
            if (containerProvider is IContainerRegistry containerRegistry)
                return containerRegistry.IsRegistered(type, name);
            return false;
        }
    }
}
