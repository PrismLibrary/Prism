using Autofac;
using Autofac.Features.ResolveAnything;
using System;

namespace Prism.Autofac.Ioc
{
    public class AutofacContainerExtension : IAutofacContainerExtension
    {
        public ContainerBuilder Builder { get; }

        public IContainer Instance { get; private set; }

        public bool SupportsModules => false;

        public AutofacContainerExtension()
            : this(new ContainerBuilder()) { }

        public AutofacContainerExtension(IContainer container)
        {
            Instance = container;
        }

        public AutofacContainerExtension(ContainerBuilder builder)
        {
            Builder = builder;
        }

        public void FinalizeExtension()
        {
            if (Instance != null)
                return;

            // Make sure any not specifically registered concrete type can resolve.
            Builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            Instance = Builder.Build();
        }

        public void RegisterInstance(Type type, object instance)
        {
            Builder.RegisterInstance(instance).As(type);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Builder.RegisterType(to).As(from).SingleInstance();
        }

        public void Register(Type from, Type to)
        {
            Builder.RegisterType(to).As(from);
        }

        public void Register(Type from, Type to, string name)
        {
            Builder.RegisterType(to).Named(name, from);
        }

        public object Resolve(Type type)
        {
            return Instance.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.ResolveNamed(name, type);
        }

        public object ResolveViewModelForView(object view, Type viewModelType)
        {
            return Instance.Resolve(viewModelType);
        }
    }
}
