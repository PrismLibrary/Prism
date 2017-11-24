using System;
using Prism.Ioc;
using Autofac;
using Autofac.Features.ResolveAnything;

namespace Prism.Autofac
{
    public class AutofacContainerExtension : IAutofacContainerExtension
    {
        public AutofacContainerExtension(ContainerBuilder builder)
        {
            Builder = builder;
        }

        public ContainerBuilder Builder { get; }

        public IContainer Instance { get; private set; }

        public void Finalize()
        {
            // Make sure any not specifically registered concrete type can resolve.
            Builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            Instance = Builder.Build();
        }

        public void RegisterInstance<TInterface>(TInterface instance)
            where TInterface : class
        {
            Builder.RegisterInstance(instance).As<TInterface>().SingleInstance();
        }

        public void RegisterSingleton<T>()
        {
            Builder.RegisterType<T>().As<T>().SingleInstance();
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
        {
            Builder.RegisterType<TTo>().As<TFrom>().SingleInstance();
        }

        public void RegisterType(Type from, Type to)
        {
            Builder.RegisterType(to).As(from);
        }

        public void RegisterType(Type from, Type to, string name)
        {
            Builder.RegisterType(to).Named(name, from);
        }

        public void RegisterType<T>()
        {
            Builder.RegisterType<T>().As<T>();
        }

        public void RegisterType<T>(string name)
        {
            Builder.RegisterType<T>().Named<T>(name);
        }

        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            Builder.RegisterType<TTo>().As<TFrom>();
        }

        public void RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            Builder.RegisterType<TTo>().Named<TFrom>(name);
        }

        public object Resolve(Type type)
        {
            return Instance.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.ResolveNamed(name, type);
        }

        public T Resolve<T>()
        {
            return Instance.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            return Instance.ResolveNamed<T>(name);
        }
    }
}
