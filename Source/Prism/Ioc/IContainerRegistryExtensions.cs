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

        public static void Register(this IContainerRegistry containerRegistry, Type type)
        {
            containerRegistry.Register(type, type);
        }

        public static void Register<T>(this IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(T));
        }

        public static void Register(this IContainerRegistry containerRegistry, Type type, string name)
        {
            containerRegistry.Register(type, type, name);
        }

        public static void Register<T>(this IContainerRegistry containerRegistry, string name)
        {
            containerRegistry.Register(typeof(T), name);
        }

        public static void Register<TFrom, TTo>(this IContainerRegistry containerRegistry) where TTo : TFrom
        {
            containerRegistry.Register(typeof(TFrom), typeof(TTo));
        }

        public static void Register<TFrom, TTo>(this IContainerRegistry containerRegistry, string name) where TTo : TFrom
        {
            containerRegistry.Register(typeof(TFrom), typeof(TTo), name);
        }
    }
}
