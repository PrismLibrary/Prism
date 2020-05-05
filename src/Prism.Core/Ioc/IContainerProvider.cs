using System;

namespace Prism.Ioc
{
    /// <summary>
    /// Resolves Services from the Container
    /// </summary>
    public interface IContainerProvider
    {
        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        object Resolve(Type type, params (Type Type, object Instance)[] parameters);

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        object Resolve(Type type, string name);

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters);

        /// <summary>
        /// Creates a new scope
        /// </summary>
        IScopedProvider CreateScope();

        /// <summary>
        /// Gets the Current Scope
        /// </summary>
        IScopedProvider CurrentScope { get; }
    }
}
