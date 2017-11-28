using System;

namespace Prism.Ioc
{
    public static class IContainerRegistryExtensions
    {
        public static void RegisterInstance<TInterface>(this IContainerRegistry containerRegistry, TInterface instance)
        {
            containerRegistry.RegisterInstance(typeof(TInterface), instance);
        }

        public static void RegisterSingleton(this IContainerRegistry containerRegistry, Type type)
        {
            containerRegistry.RegisterSingleton(type, type);
        }

        public static void RegisterSingleton<TFrom, TTo>(this IContainerRegistry containerRegistry) where TTo : TFrom
        {
            containerRegistry.RegisterSingleton(typeof(TFrom), typeof(TTo));
        }

        public static void RegisterSingleton<T>(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton(typeof(T));
        }

        public static void RegisterType(this IContainerRegistry containerRegistry, Type type)
        {
            containerRegistry.RegisterType(type, type);
        }

        public static void RegisterType<T>(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterType(typeof(T));
        }

        public static void RegisterType(this IContainerRegistry containerRegistry, Type type, string name)
        {
            containerRegistry.RegisterType(type, type, name);
        }

        public static void RegisterType<T>(this IContainerRegistry containerRegistry, string name)
        {
            containerRegistry.RegisterType(typeof(T), name);
        }

        public static void RegisterType<TFrom, TTo>(this IContainerRegistry containerRegistry) where TTo : TFrom
        {
            containerRegistry.RegisterType(typeof(TFrom), typeof(TTo));
        }

        public static void RegisterType<TFrom, TTo>(this IContainerRegistry containerRegistry, string name) where TTo : TFrom
        {
            containerRegistry.RegisterType(typeof(TFrom), typeof(TTo), name);
        }
    }
}
