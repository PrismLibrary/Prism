using Autofac;
using System;

namespace Prism.Autofac
{
    public static class AutofacExtensions
    {
        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <param name="name">The unique name to register with the object</param>
        public static void RegisterTypeForNavigation<T>(this ContainerBuilder builder, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;
            builder.RegisterType<T>().Named<object>(viewName);
        }
    }
}
