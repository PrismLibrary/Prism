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

        public bool SupportsModules => false;

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

        public void RegisterInstance(Type type, object instance)
        {
            Builder.RegisterInstance(instance).As(type);
        }

        public void RegisterSingleton(Type type)
        {
            Builder.RegisterType(type).As(type).SingleInstance();
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Builder.RegisterType(to).As(from).SingleInstance();
        }

        public void RegisterType(Type from, Type to)
        {
            Builder.RegisterType(to).As(from);
        }

        public void RegisterType(Type from, Type to, string name)
        {
            Builder.RegisterType(to).Named(name, from);
        }

        public void RegisterType(Type type)
        {
            Builder.RegisterType(type).As(type);
        }

        public void RegisterType(Type type, string name)
        {
            Builder.RegisterType(type).Named(name, type);
        }

        public object Resolve(Type type)
        {
            return Instance.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.ResolveNamed(name, type);
        }
    }
}
