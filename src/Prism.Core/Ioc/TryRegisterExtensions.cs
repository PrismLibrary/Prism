using System;

namespace Prism.Ioc
{
    public static class TryRegisterExtensions
    {
        /// <summary>
        /// Will register the given type if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegister(this IContainerRegistry containerRegistry, Type type)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.Register(type);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegister(this IContainerRegistry containerRegistry, Type type, Type implementation)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.Register(type, implementation);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the named type if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <param name="implementation"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegister(this IContainerRegistry containerRegistry, Type type, Type implementation, string name)
        {
            if (!containerRegistry.IsRegistered(type, name))
                containerRegistry.Register(type, implementation, name);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegister(this IContainerRegistry containerRegistry, Type type, Func<object> factory)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.Register(type, factory);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegister(this IContainerRegistry containerRegistry, Type type, Func<IContainerProvider, object> factory)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.Register(type, factory);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegister<T>(this IContainerRegistry containerRegistry, Func<object> factory) =>
            containerRegistry.TryRegister(typeof(T), factory);

        /// <summary>
        /// Will register the given type if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegister<T>(this IContainerRegistry containerRegistry, Func<IContainerProvider, object> factory) =>
            containerRegistry.TryRegister(typeof(T), factory);

        /// <summary>
        /// Will register the given type if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegister<T>(this IContainerRegistry containerRegistry) =>
            containerRegistry.TryRegister(typeof(T));

        /// <summary>
        /// Will register the given type if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegister<TService, TImplementation>(this IContainerRegistry containerRegistry)
            where TImplementation : TService =>
            containerRegistry.TryRegister(typeof(TService), typeof(TImplementation));

        /// <summary>
        /// Will register the named type if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegister<TService, TImplementation>(this IContainerRegistry containerRegistry, string name)
            where TImplementation : TService =>
            containerRegistry.TryRegister(typeof(TService), typeof(TImplementation), name);

        /// <summary>
        /// Will register the given type as a singleton if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterSingleton(this IContainerRegistry containerRegistry, Type type)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.RegisterSingleton(type);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type as a singleton if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterSingleton(this IContainerRegistry containerRegistry, Type type, Type implementation)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.RegisterSingleton(type, implementation);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type as a singleton if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterSingleton(this IContainerRegistry containerRegistry, Type type, Func<object> factory)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.RegisterSingleton(type, factory);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type as a singleton if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterSingleton(this IContainerRegistry containerRegistry, Type type, Func<IContainerProvider, object> factory)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.RegisterSingleton(type, factory);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type as a singleton if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterSingleton<T>(this IContainerRegistry containerRegistry, Func<object> factory) =>
            containerRegistry.TryRegisterSingleton(typeof(T), factory);

        /// <summary>
        /// Will register the given type as a singleton if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterSingleton<T>(this IContainerRegistry containerRegistry, Func<IContainerProvider, object> factory) =>
            containerRegistry.TryRegisterSingleton(typeof(T), factory);

        /// <summary>
        /// Will register the given type as a singleton if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterSingleton<T>(this IContainerRegistry containerRegistry) =>
            containerRegistry.RegisterSingleton(typeof(T));

        /// <summary>
        /// Will register the given type as a singleton if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterSingleton<TService, TImplementation>(this IContainerRegistry containerRegistry)
            where TImplementation : TService =>
            containerRegistry.RegisterSingleton(typeof(TService), typeof(TImplementation));

        /// <summary>
        /// Will register the given type as a scoped if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterScoped(this IContainerRegistry containerRegistry, Type type)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.RegisterScoped(type);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type as a scoped if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterScoped(this IContainerRegistry containerRegistry, Type type, Type implementation)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.RegisterScoped(type, implementation);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type as a scoped if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterScoped(this IContainerRegistry containerRegistry, Type type, Func<object> factory)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.RegisterScoped(type, factory);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type as a scoped if it has not already been registered with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterScoped(this IContainerRegistry containerRegistry, Type type, Func<IContainerProvider, object> factory)
        {
            if (!containerRegistry.IsRegistered(type))
                containerRegistry.RegisterScoped(type, factory);
            return containerRegistry;
        }

        /// <summary>
        /// Will register the given type as a scoped if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterScoped<T>(this IContainerRegistry containerRegistry, Func<object> factory) =>
            containerRegistry.TryRegisterScoped(typeof(T), factory);

        /// <summary>
        /// Will register the given type as a scoped if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterScoped<T>(this IContainerRegistry containerRegistry, Func<IContainerProvider, object> factory) =>
            containerRegistry.TryRegisterScoped(typeof(T), factory);

        /// <summary>
        /// Will register the given type as a scoped if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterScoped<T>(this IContainerRegistry containerRegistry) =>
            containerRegistry.TryRegisterScoped(typeof(T));

        /// <summary>
        /// Will register the given type as a scoped if it has not already been registered with the container.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="containerRegistry"></param>
        /// <returns></returns>
        public static IContainerRegistry TryRegisterScoped<TService, TImplementation>(this IContainerRegistry containerRegistry)
            where TImplementation : TService =>
            containerRegistry.TryRegisterScoped(typeof(TService), typeof(TImplementation));
    }
}
