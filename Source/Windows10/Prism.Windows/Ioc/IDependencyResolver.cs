using System;

namespace Prism.Ioc
{
    public interface IDependencyResolver
    {
        object Resolve(Type serviceType, params (Type resolvingType, object instance)[] args);
    }

    public static class IDependencyResolverExtensions
    {
        public static T Resolve<T>(this IDependencyResolver resolver, params (Type resolvingType, object instance)[] args) =>
            (T)resolver.Resolve(typeof(T), args);
    }
}
