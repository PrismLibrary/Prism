using System;
using Microsoft.Practices.Unity;

namespace Prism.Unity
{
    public static class UnityContainerExtensions
    {
        /// <summary>
        /// Registers an object for navigation using a convention based approach, which uses the name of the Type being passed in as the unique name.
        /// </summary>
        /// <typeparam name="T">The Type of Page to register</typeparam>
        public static void RegisterTypeForNavigation<T>(this IUnityContainer container)
        {
            Type type = typeof(T);
            container.RegisterType(typeof(object), type, type.Name);
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <param name="name">The unique name to register with the object</param>
        public static void RegisterTypeForNavigation<T>(this IUnityContainer container, string name)
        {
            container.RegisterType(typeof(object), typeof(T), name);
        }
    }
}
