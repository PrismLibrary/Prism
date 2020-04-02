using System;

namespace Prism.Ioc
{
    public static class IContainerProviderExtensions
    {
        public static T Resolve<T>(this IContainerProvider provider)
        {
            return (T)provider.Resolve(typeof(T));
        }

        public static T Resolve<T>(this IContainerProvider provider, params (Type Type, object Instance)[] parameters)
        {
            return (T)provider.Resolve(typeof(T), parameters);
        }

        public static T Resolve<T>(this IContainerProvider provider, string name, params (Type Type, object Instance)[] parameters)
        {
            return (T)provider.Resolve(typeof(T), name, parameters);
        }

        public static T Resolve<T>(this IContainerProvider provider, string name)
        {
            return (T)provider.Resolve(typeof(T), name);
        }
    }
}
