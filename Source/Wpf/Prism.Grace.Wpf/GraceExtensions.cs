using System;
using Prism.Mvvm;
using Grace.DependencyInjection;

namespace Prism.Grace
{
    public static class GraceExtensions
    {
        /// <summary>
        /// Registers an object for navigation
        /// </summary>
        /// <param name="container"></param>
        /// <param name="type">The type of object to register</param>
        /// <param name="name">The unique name to register with the obect.</param>
        /// <returns><see cref="DependencyInjectionContainer"/></returns>
        public static DependencyInjectionContainer ConfigureTypeForNavigation(this DependencyInjectionContainer container, Type type, string name)
        {
            container.Configure(c => c.Export(type).As(typeof(object)).AsName(name));

            return container;
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register as the view</typeparam>
        /// <param name="container"></param>
        /// <param name="name">The unique name to register with the object.</param>
        public static DependencyInjectionContainer RegisterTypeForNavigation<T>(this DependencyInjectionContainer container, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;
            return container.ConfigureTypeForNavigation(type, viewName);
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of object to register as the view</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the DataContext for the view</typeparam>
        /// <param name="name">The unique name to register with the view</param>
        /// <param name="container"></param>
        public static DependencyInjectionContainer RegisterTypeForNavigation<TView, TViewModel>(this DependencyInjectionContainer container, string name = null)
        {
            return container.ConfigureTypeForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        private static DependencyInjectionContainer ConfigureTypeForNavigationWithViewModel<TViewModel>(this DependencyInjectionContainer container, Type viewType, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            return container.ConfigureTypeForNavigation(viewType, name);
        }
    }
}
