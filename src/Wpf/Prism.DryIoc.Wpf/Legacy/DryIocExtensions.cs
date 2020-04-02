using DryIoc;
using Prism.Mvvm;
using System;

namespace Prism.DryIoc
{
    public static class DryIocExtensions
    {
        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <param name="name">The unique name to register with the object</param>
        public static void RegisterTypeForNavigation<T>(this IContainer container, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;
            container.RegisterTypeForNavigation(type, viewName);
        }

        /// <summary>
        /// Registers an object for navigation
        /// </summary>
        /// <param name="container"></param>
        /// <param name="type">The type of object to register</param>
        /// <param name="name">The unique name to register with the obect.</param>
        public static void RegisterTypeForNavigation(this IContainer container, Type type, string name)
        {
            container.Register(typeof(object),
                               type,
                               made: Made.Of(FactoryMethod.ConstructorWithResolvableArguments),
                               serviceKey: name);
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of object to register as the view</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the DataContext for the view</typeparam>
        /// <param name="name">The unique name to register with the view</param>
        /// <param name="container"></param>
        public static void RegisterTypeForNavigation<TView, TViewModel>(this IContainer container, string name = null)
        {
            container.RegisterTypeForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        private static void RegisterTypeForNavigationWithViewModel<TViewModel>(this IContainer container, Type viewType, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            container.RegisterTypeForNavigation(viewType, name);
        }
    }
}
