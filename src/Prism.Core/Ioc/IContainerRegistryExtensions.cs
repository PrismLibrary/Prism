using System;

namespace Prism.Ioc
{
    public static class IContainerRegistryExtensions
    {
        public static IContainerRegistry RegisterInstance<TInterface>(this IContainerRegistry containerRegistry, TInterface instance)
        {
            return containerRegistry.RegisterInstance(typeof(TInterface), instance);
        }

        public static IContainerRegistry RegisterInstance<TInterface>(this IContainerRegistry containerRegistry, TInterface instance, string name)
        {
            return containerRegistry.RegisterInstance(typeof(TInterface), instance, name);
        }

        public static IContainerRegistry RegisterSingleton(this IContainerRegistry containerRegistry, Type type)
        {
            return containerRegistry.RegisterSingleton(type, type);
        }

        public static IContainerRegistry RegisterSingleton<TFrom, TTo>(this IContainerRegistry containerRegistry) where TTo : TFrom
        {
            return containerRegistry.RegisterSingleton(typeof(TFrom), typeof(TTo));
        }

        public static IContainerRegistry RegisterSingleton<TFrom, TTo>(this IContainerRegistry containerRegistry, string name) where TTo : TFrom
        {
            return containerRegistry.RegisterSingleton(typeof(TFrom), typeof(TTo), name);
        }

        public static IContainerRegistry RegisterSingleton<T>(this IContainerRegistry containerRegistry)
        {
            return containerRegistry.RegisterSingleton(typeof(T));
        }

        public static IContainerRegistry Register(this IContainerRegistry containerRegistry, Type type)
        {
            return containerRegistry.Register(type, type);
        }

        public static IContainerRegistry Register<T>(this IContainerRegistry containerRegistry)
        {
            return containerRegistry.Register(typeof(T));
        }

        public static IContainerRegistry Register(this IContainerRegistry containerRegistry, Type type, string name)
        {
            return containerRegistry.Register(type, type, name);
        }

        public static IContainerRegistry Register<T>(this IContainerRegistry containerRegistry, string name)
        {
            return containerRegistry.Register(typeof(T), name);
        }

        public static IContainerRegistry Register<TFrom, TTo>(this IContainerRegistry containerRegistry) where TTo : TFrom
        {
            return containerRegistry.Register(typeof(TFrom), typeof(TTo));
        }

        public static IContainerRegistry Register<TFrom, TTo>(this IContainerRegistry containerRegistry, string name) where TTo : TFrom
        {
            return containerRegistry.Register(typeof(TFrom), typeof(TTo), name);
        }

        public static bool IsRegistered<T>(this IContainerRegistry containerRegistry)
        {
            return containerRegistry.IsRegistered(typeof(T));
        }

        public static bool IsRegistered<T>(this IContainerRegistry containerRegistry, string name)
        {
            return containerRegistry.IsRegistered(typeof(T), name);
        }
    }
}
