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
    }
}
