using System;
using System.ComponentModel;

namespace Prism.Ioc.Internals
{
    /// <summary>
    /// Internal extensions to get the registered implementation for Regions
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class IContainerInfoExtensions
    {
        /// <summary>
        /// Locates the registered implementation <see cref="Type"/> for a give key
        /// </summary>
        /// <param name="container">The <see cref="IContainerExtension"/></param>
        /// <param name="key">Registration Key</param>
        /// <returns>Implementation <see cref="Type"/></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Type GetRegistrationType(this IContainerExtension container, string key)
        {
            if (container is IContainerInfo ci)
                return ci.GetRegistrationType(key);

            return null;
        }

        /// <summary>
        /// Locates the registered implementation <see cref="Type"/> for a give key
        /// </summary>
        /// <param name="container">The <see cref="IContainerExtension"/></param>
        /// <param name="type">Service Type</param>
        /// <returns>Implementation <see cref="Type"/></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Type GetRegistrationType(this IContainerExtension container, Type
            type)
        {
            if (container is IContainerInfo ci)
                return ci.GetRegistrationType(type);

            return null;
        }
    }
}
