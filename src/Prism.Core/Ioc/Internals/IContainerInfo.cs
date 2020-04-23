using System;
using System.ComponentModel;

namespace Prism.Ioc.Internals
{
    /// <summary>
    /// Used to resolve the registered implementation type for a given key
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IContainerInfo
    {
        /// <summary>
        /// Locates the registered implementation <see cref="Type"/> for a give key
        /// </summary>
        /// <param name="key">Registration Key</param>
        /// <returns>Implementation <see cref="Type"/></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetRegistrationType(string key);

        /// <summary>
        /// Locates the registered implementation <see cref="Type"/> for a give key
        /// </summary>
        /// <param name="serviceType">Service Type</param>
        /// <returns>Implementation <see cref="Type"/></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetRegistrationType(Type serviceType);
    }
}
