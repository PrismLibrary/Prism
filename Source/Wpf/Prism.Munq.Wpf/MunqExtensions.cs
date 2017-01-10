using System;
using Munq;

namespace Prism.Munq
{
    public static class MunqExtensions
    {
        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <param name="container"><see cref="IDependecyRegistrar"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the object.</param>
        public static IDependecyRegistrar RegisterTypeForNavigation<T>(this IDependecyRegistrar container, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;

            container.Register(viewName, typeof(object), type);

            return container;
        }
    }
}
