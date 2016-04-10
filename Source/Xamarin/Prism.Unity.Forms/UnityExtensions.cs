using System;
using Microsoft.Practices.Unity;
using Xamarin.Forms;
using Prism.Navigation;
using Prism.Common;

namespace Prism.Unity
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Registers a Page for navigation using a convention based approach, which uses the name of the Type being passed in as the unique name.
        /// </summary>
        /// <typeparam name="T">The Type of Page to register</typeparam>
        public static void RegisterTypeForNavigation<T>(this IUnityContainer container) where T : Page
        {
            container.RegisterTypeForNavigation<T>(typeof(T).Name);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of Page to register</typeparam>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterTypeForNavigation<T>(this IUnityContainer container, string name) where T : Page
        {
            Type type = typeof(T);

            container.RegisterType(typeof(object), type, name);

            PageNavigationRegistry.Register(name, type);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of Page to register</typeparam>
        /// <typeparam name="C">The Class to use as the unique name for the Page</typeparam>
        /// <param name="container"></param>
        public static void RegisterTypeForNavigation<T, C>(this IUnityContainer container)
            where T : Page
            where C : class
        {
            Type type = typeof(T);
            string name = typeof(C).FullName;

            container.RegisterType(typeof(object), type, name);

            PageNavigationRegistry.Register(name, type);
        }
    }
}
