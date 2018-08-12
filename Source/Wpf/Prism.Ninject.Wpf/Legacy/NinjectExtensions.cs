using System;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using Prism.Mvvm;

namespace Prism.Ninject
{
    public static class NinjectExtensions
    {
        public static bool IsRegistered<TService>(this IKernel kernel)
        {
            return kernel.IsRegistered(typeof(TService));
        }

        public static bool IsRegistered(this IKernel kernel, Type type)
        {
            return kernel.CanResolve(kernel.CreateRequest(type, _ => true, new IParameter[] { }, false, false));
        }

        public static void RegisterTypeIfMissing<TFrom, TTo>(this IKernel kernel, bool asSingleton)
        {
            kernel.RegisterTypeIfMissing(typeof(TFrom), typeof(TTo), asSingleton);
        }

        public static void RegisterTypeIfMissing(this IKernel kernel, Type from, Type to, bool asSingleton)
        {
            // Don't do anything if there are already bindings registered
            if (kernel.IsRegistered(from))
            {
                return;
            }

            // Register the types
            var binding = kernel.Bind(from).To(to);
            if (asSingleton)
            {
                binding.InSingletonScope();
            }
            else
            {
                binding.InTransientScope();
            }
        }

        /// <summary>
        /// Registers an object for navigation
        /// </summary>
        /// <param name="container"></param>
        /// <param name="type">The type of object to register</param>
        /// <param name="name">The unique name to register with the obect.</param>
        public static void RegisterTypeForNavigation(this IKernel container, Type type, string name)
        {
            container.Bind<object>().To(type).Named(name);
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <param name="ninjectModule"><see cref="NinjectModule"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the object.</param>
        public static void RegisterTypeForNavigation<T>(this NinjectModule ninjectModule, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;
            ninjectModule.Bind<object>().To<T>().Named(viewName);
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <param name="kernel"><see cref="IKernel"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the object.</param>
        public static void RegisterTypeForNavigation<T>(this IKernel kernel, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;
            kernel.RegisterTypeForNavigation(type, viewName);
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of object to register as the view</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the DataContext for the view</typeparam>
        /// <param name="name">The unique name to register with the view</param>
        /// <param name="container"></param>
        public static void RegisterTypeForNavigation<TView, TViewModel>(this IKernel container, string name = null)
        {
            container.RegisterTypeForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        private static void RegisterTypeForNavigationWithViewModel<TViewModel>(this IKernel container, Type viewType, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            container.RegisterTypeForNavigation(viewType, name);
        }
    }
}