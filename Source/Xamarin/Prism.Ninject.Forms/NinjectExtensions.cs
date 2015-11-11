using System;
using Ninject;
using Xamarin.Forms;
using Prism.Common;

namespace Prism.Ninject
{
    public static class NinjectExtensions
    {
        /// <summary>
        /// Registers a Page for navigation using a convention based approach, which uses the name of the Type being passed in as the unique name.
        /// </summary>
        /// <typeparam name="T">The Type of Page to register</typeparam>
        public static void RegisterTypeForNavigation<T>(this IKernel kernel) where T : Page
        {
            kernel.RegisterTypeForNavigation<T>(typeof(T).Name);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of Page to register</typeparam>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterTypeForNavigation<T>(this IKernel kernel, string name) where T : Page
        {
            Type type = typeof(T);
            kernel.Bind<object>().To(type).Named(name);

            PageNavigationRegistry.Register(name, type);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of Page to register</typeparam>
        /// <typeparam name="C">The Class to use as the unique name for the Page</typeparam>
        /// <param name="kernel"></param>
        public static void RegisterTypeForNavigation<T, C>(this IKernel kernel)
            where T : Page
            where C : class
        {
            Type type = typeof(T);
            string name = typeof(C).FullName;

            kernel.Bind<object>().To(type).Named(name);

            PageNavigationRegistry.Register(name, type);
        }
    }
}
