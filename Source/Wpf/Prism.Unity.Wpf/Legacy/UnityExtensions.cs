using Unity;
using System;
using Prism.Mvvm;

namespace Prism.Unity
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Registers an object for navigation
        /// </summary>
        /// <param name="container"></param>
        /// <param name="type">The type of object to register</param>
        /// <param name="name">The unique name to register with the obect.</param>
        /// <returns><see cref="IUnityContainer"/></returns>
        public static IUnityContainer RegisterTypeForNavigation(this IUnityContainer container, Type type, string name)
        {
            return container.RegisterType(typeof(object), type, name);
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register as the view</typeparam>
        /// <param name="container"></param>
        /// <param name="name">The unique name to register with the object.</param>
        public static IUnityContainer RegisterTypeForNavigation<T>(this IUnityContainer container, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;
            return container.RegisterTypeForNavigation(type, viewName);
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of object to register as the view</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the DataContext for the view</typeparam>
        /// <param name="name">The unique name to register with the view</param>
        /// <param name="container"></param>
        public static IUnityContainer RegisterTypeForNavigation<TView, TViewModel>(this IUnityContainer container, string name = null)
        {
            return container.RegisterTypeForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        private static IUnityContainer RegisterTypeForNavigationWithViewModel<TViewModel>(this IUnityContainer container, Type viewType, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            return container.RegisterTypeForNavigation(viewType, name);
        }
    }
}
