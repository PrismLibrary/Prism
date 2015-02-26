using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;
using Xamarin.Forms;

namespace Prism.Unity
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Registers a Page for navigation using the name parameter.
        /// </summary>
        /// <typeparam name="T">The type of Page to register</typeparam>
        /// <param name="name">The unique name to associate with the registered Page</param>
        public static void RegisterTypeForNavigation<T>(this IUnityContainer container, string name) where T : Page
        {
            container.RegisterType(typeof(object), typeof(T), name);
        }

        /// <summary>
        /// Registers a Page for navigation using a convention based approach, using the name of the type being passed in.
        /// </summary>
        /// <typeparam name="T">The type of Page to register</typeparam>
        public static void RegisterTypeForNavigation<T>(this IUnityContainer container) where T : Page
        {
            Type type = typeof (T);
            container.RegisterType(typeof(object), type, type.Name);
        }
    }
}
