using Grace.DependencyInjection;
using Prism.Ioc;
using System;

namespace Prism.Grace.Ioc
{
    public class GraceContainerExtension : IContainerExtension<DependencyInjectionContainer>
    {
        public DependencyInjectionContainer Instance { get; }

        public bool SupportsModules => true;

        public GraceContainerExtension() : this(new DependencyInjectionContainer()) { }

        public GraceContainerExtension(DependencyInjectionContainer container) => Instance = container;

        public void FinalizeExtension() { }

        public void RegisterInstance(Type type, object instance)
        {
            Instance.Configure(c => c.ExportInstance(instance).As(type));
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Instance.Configure(c => c.Export(to).As(from).Lifestyle.Singleton());
        }

        public void Register(Type from, Type to)
        {
            Instance.Configure(c => c.Export(to).As(from));
        }

        public void Register(Type from, Type to, string name)
        {
            Instance.Configure(c => c.Export(to).AsKeyed(from, name));
        }

        public object Resolve(Type type)
        {
            return Instance.Locate(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.Locate(type, withKey: name);
        }

        public object ResolveViewModelForView(object view, Type viewModelType)
        {
            return Instance.Locate(viewModelType);
        }
    }
}
