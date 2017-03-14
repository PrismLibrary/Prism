using Microsoft.Practices.Unity;
using System;

namespace Prism.Unity
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <param name="container"><see cref="IUnityContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the object.</param>
        public static IUnityContainer RegisterTypeForNavigation<T>(this IUnityContainer container, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;
            return container.RegisterType(typeof(object), type, viewName);
        }
    }
}
