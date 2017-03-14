using System;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;

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
            kernel.Bind<object>().To<T>().Named(viewName);
        }
    }
}