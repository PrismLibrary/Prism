using Prism.Ioc;

namespace Prism
{
    /// <summary>
    /// IPlatformInitializer is used when there are specific types that need to be registered on a platform.
    /// </summary>
    public interface IPlatformInitializer
    {
        /// <summary>
		/// Registers platform specific types
		/// </summary>
        /// <example>
        /// <see cref="RegisterTypes(IContainerRegistry)"/>
        /// </example>
        void RegisterTypes(IContainerRegistry containerRegistry);
    }
}
