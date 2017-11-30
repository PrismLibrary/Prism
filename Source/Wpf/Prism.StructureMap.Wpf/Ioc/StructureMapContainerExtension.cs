using Prism.Ioc;
using StructureMap;
using System;

namespace Prism.StructureMap.Ioc
{
    public class StructureMapContainerExtension : IContainerExtension<IContainer>
    {
        public IContainer Instance { get; }

        public bool SupportsModules => true;

        public StructureMapContainerExtension()
            : this(new Container()) { }

        public StructureMapContainerExtension(IContainer container) => Instance = container;

        public void FinalizeExtension() { }

        public void Register(Type from, Type to)
        {
            Instance.Configure(config => config.For(from).Use(to));
        }

        public void Register(Type from, Type to, string name)
        {
            Instance.Configure(config => config.For(from).Use(to).Name = name);
        }

        public void RegisterInstance(Type type, object instance)
        {
            Instance.Configure(config => config.For(type).Use(instance));
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Instance.Configure(config => config.For(from).Singleton().Use(to));
        }

        public object Resolve(Type type)
        {
            return Instance.GetInstance(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.GetInstance(type, name);
        }

        public object ResolveViewModelForView(object view, Type viewModelType)
        {
            return Instance.GetInstance(viewModelType);
        }
    }
}
