using Autofac;
using Prism.Mvvm;
using System;

namespace Prism.Autofac
{
    public static class AutofacExtensions
    {
        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <param name="builder"><see cref="ContainerBuilder"/> used to build <see cref="IContainer"/></param>
        /// <param name="name">The unique name to register with the object</param>
        public static void RegisterTypeForNavigation<T>(this ContainerBuilder builder, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;
            builder.RegisterTypeForNavigation(type, viewName);
        }

        /// <summary>
        /// Registers an object for navigation
        /// </summary>
        /// <param name="container"></param>
        /// <param name="type">The type of object to register</param>
        /// <param name="name">The unique name to register with the obect.</param>
        public static void RegisterTypeForNavigation(this ContainerBuilder builder, Type type, string name)
        {
            builder.RegisterType(type).Named<object>(name);
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of object to register as the view</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the DataContext for the view</typeparam>
        /// <param name="name">The unique name to register with the view</param>
        public static void RegisterTypeForNavigation<TView, TViewModel>(this ContainerBuilder builder, string name = null)
        {
            builder.RegisterTypeForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        private static void RegisterTypeForNavigationWithViewModel<TViewModel>(this ContainerBuilder builder, Type viewType, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            builder.RegisterTypeForNavigation(viewType, name);
        }
    }
}
