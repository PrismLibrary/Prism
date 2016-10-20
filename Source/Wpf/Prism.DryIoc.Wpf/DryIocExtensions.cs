using DryIoc;
using System;

namespace Prism.DryIoc
{
    public static class DryIocExtensions
    {
        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <param name="builder"><see cref="ContainerBuilder"/> used to build <see cref="IContainer"/></param>
        /// <param name="name">The unique name to register with the object</param>
        public static void RegisterTypeForNavigation<T>(this IContainer container, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;
            container.Register(type, serviceKey: viewName);
        }
    }
}
