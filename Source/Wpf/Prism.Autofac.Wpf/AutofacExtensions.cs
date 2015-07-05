using Autofac;

namespace Prism.Autofac
{
    public static class AutofacExtensions
    {
        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <param name="name">The unique name to register with the object</param>
        public static void RegisterTypeForNavigation<T>(this ContainerBuilder builder, string name)
        {
            builder.RegisterType<T>().Named<object>(name);
        }
    }
}
