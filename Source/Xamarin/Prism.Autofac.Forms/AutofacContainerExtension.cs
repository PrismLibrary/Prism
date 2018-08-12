using Autofac;
using Autofac.Core;
using Autofac.Features.ResolveAnything;
using Prism.Ioc;
using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace Prism.Autofac
{
    public class AutofacContainerExtension : IAutofacContainerExtension
    {
        public ContainerBuilder Builder { get; }

        public IContainer Instance { get; private set; }

        public bool SupportsModules => false;

        public AutofacContainerExtension(ContainerBuilder builder)
        {
            Builder = builder;
        }

        public void FinalizeExtension()
        {
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
            Parameter parameter = null;
            if (view is Page page)
            {
                parameter = new TypedParameter(typeof(INavigationService), this.CreateNavigationService(page));
            }

            return Instance.Resolve(viewModelType, parameter);
        }
    }
}
