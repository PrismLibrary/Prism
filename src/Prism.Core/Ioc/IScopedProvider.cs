using System;

namespace Prism.Ioc
{
    /// <summary>
    /// Defines a Container Scope
    /// </summary>
    public interface IScopedProvider : IContainerProvider, IDisposable
    {
        /// <summary>
        /// Gets or Sets the IsAttached property.
        /// </summary>
        /// <remarks>
        /// Indicates that Prism is tracking the scope
        /// </remarks>
        bool IsAttached { get; set; }
    }
}
