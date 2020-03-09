using Prism.Ioc;

namespace Prism
{
    /// <summary>
    /// IPlatformInitializer Interface
    /// </summary>
    /// <example> 
    /// IPlatformInitializer is used when there are specific types that need to be registered on a platform.
    /// </example>
    public interface IPlatformInitializer
    {
        /// <summary>
		/// Registers platform specific types
		/// </summary>
        /// <example> 
        /// <see cref="RegisterTypes(IContainerRegistry)"/>
        /// <code>
        /// </code>
        /// </example>
        void RegisterTypes(IContainerRegistry containerRegistry);
    }
}