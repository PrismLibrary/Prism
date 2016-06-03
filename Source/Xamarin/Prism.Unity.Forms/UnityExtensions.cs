using System;
using Microsoft.Practices.Unity;
using Xamarin.Forms;
using Prism.Common;
using Prism.Mvvm;

namespace Prism.Unity
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Registers a Page for navigation using a convention based approach, which uses the name of the Type being passed in as the unique name.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        public static void RegisterTypeForNavigation<TView>(this IUnityContainer container) where TView : Page
        {
            container.RegisterTypeForNavigation<TView>(typeof(TView).Name);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <param name="container"><see cref="IUnityContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterTypeForNavigation<TView>(this IUnityContainer container, string name) where TView : Page
        {
            Type type = typeof(TView);

            container.RegisterType(typeof(object), type, name);

            PageNavigationRegistry.Register(name, type);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <typeparam name="TViewModel">The BindableBase ViewModel to use as the unique name for the Page</typeparam>
        /// <param name="container"></param>
        public static void RegisterTypeForNavigation<TView, TViewModel>(this IUnityContainer container)
            where TView : Page
            where TViewModel : BindableBase
        {
            Type type = typeof(TView);
            string name = typeof(TViewModel).FullName;

            container.RegisterType(typeof(object), type, name);

            PageNavigationRegistry.Register(name, type);
        }
    }
}
