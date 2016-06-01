using System;
using StructureMap;

namespace Prism.StructureMap
{
    public static class StructureMapExtensions
    {
        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <param name="container"><see cref="IContainer"/> used to register type for Navigation</param>
        /// <param name="name">The unique name to register with the object</param>
        public static void RegisterTypeForNavigation<T>(this IContainer container, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;

            container.Configure(config =>
            {
                config.For<object>().Use<T>().Name = viewName;
            });
        }

        public static bool IsRegistered<TService>(this IContainer container)
        {
            return container.IsRegistered(typeof(TService));
        }

        public static bool IsRegistered(this IContainer container, Type type)
        {
            return container.Model.HasImplementationsFor(type);
        }

        public static void RegisterTypeIfMissing<TFrom, TTo>(this IContainer container, bool asSingleton)
        {
            container.RegisterTypeIfMissing(typeof(TFrom), typeof(TTo), asSingleton);
        }

        public static void RegisterTypeIfMissing(this IContainer container, Type from, Type to, bool asSingleton)
        {
            // Don't do anything if there are already bindings registered
            if (container.IsRegistered(from))
            {
                return;
            }

            // Register the types
            if (asSingleton)
            {
                container.Configure(config => config.For(from).Singleton().Use(to));
            }
            else
            {
                container.Configure(config => config.For(from).Use(to));
            }
        }
    }
}