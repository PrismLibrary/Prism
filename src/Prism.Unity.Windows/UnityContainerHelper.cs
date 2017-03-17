using System;
using Microsoft.Practices.Unity;

namespace Prism.Unity.Windows
{
    /// <summary>
    /// Extensions methods to extend and facilitate the usage of <see cref="IUnityContainer"/>.
    /// </summary>
    public static class UnityContainerHelper
    {
        /// <summary>
        /// Returns whether a specified type has a type mapping registered in the container.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/> to check for the type mapping.</param>
        /// <param name="type">The type to check if there is a type mapping for.</param>
        /// <returns><see langword="true"/> if there is a type mapping registered for <paramref name="type"/>.</returns>
        /// <remarks>In order to use this extension method, you first need to add the
        /// <see cref="IUnityContainer"/> extension to the <see cref="PrismUnityExtension"/>.
        /// </remarks>        
        public static bool IsTypeRegistered(this IUnityContainer container, Type type)
        {
            return PrismUnityExtension.IsTypeRegistered(container, type);
        }

        /// <summary>
        /// Utility method to try to resolve a service from the container avoiding an exception if the container cannot build the type.
        /// </summary>
        /// <param name="container">The cointainer that will be used to resolve the type.</param>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>The instance of <typeparamref name="T"/> built up by the container.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T TryResolve<T>(this IUnityContainer container)
        {
            object result = TryResolve(container, typeof(T));
            if (result != null)
            {
                return (T)result;
            }
            return default(T);
        }

        /// <summary>
        /// Utility method to try to resolve a service from the container avoiding an exception if the container cannot build the type.
        /// </summary>
        /// <param name="container">The cointainer that will be used to resolve the type.</param>
        /// <param name="typeToResolve">The type to resolve.</param>
        /// <returns>The instance of <paramref name="typeToResolve"/> built up by the container.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static object TryResolve(this IUnityContainer container, Type typeToResolve)
        {
            try
            {
                return container.Resolve(typeToResolve);
            }
            catch
            {
                return null;
            }
        }
    }
}