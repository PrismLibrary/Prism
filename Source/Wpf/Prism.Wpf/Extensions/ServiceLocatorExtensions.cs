

using System;

namespace Microsoft.Practices.ServiceLocation
{
    /// <summary>
    /// Defines extension methods for the <see cref="ServiceLocator"/> class.
    /// </summary>
    public static class ServiceLocatorExtensions
    {
        /// <summary>
        /// Attempts to resolve specified type from the underlying <see cref="IServiceLocator"/>.
        /// </summary>
        /// <remarks>
        /// This will return null on any <see cref="ActivationException"/>.</remarks>
        /// <param name="locator">Locator to use in resolving.</param>
        /// <param name="type">Type to resolve.</param>
        /// <returns>T or null</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locator"/> is <see langword="null"/>.</exception>
        public static object TryResolve(this IServiceLocator locator, Type type)
        {
            if (locator == null) throw new ArgumentNullException(nameof(locator));

            try
            {
                return locator.GetInstance(type);
            }
            catch (ActivationException)
            {
                return null;
            }
        }

        /// <summary>
        /// Attempts to resolve specified type from the underlying <see cref="IServiceLocator"/>.
        /// </summary>
        /// <remarks>
        /// This will return null on any <see cref="ActivationException"/>.</remarks>
        /// <typeparam name="T">Type to resolve.</typeparam>
        /// <param name="locator">Locator to use in resolving.</param>
        /// <returns>T or null</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T TryResolve<T>(this IServiceLocator locator) where T : class
        {
            return locator.TryResolve(typeof(T)) as T;
        }
    }
}
