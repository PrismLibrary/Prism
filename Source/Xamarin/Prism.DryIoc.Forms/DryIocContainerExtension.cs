using Prism.Ioc;
using DryIoc;
using System;

namespace Prism.DryIoc
{
    public class DryIocContainerExtension : IContainerExtension<IContainer>
    {
        public IContainer Instance { get; }

        public bool SupportsModules => true;

        public DryIocContainerExtension(IContainer container)
        {
            Instance = container;
        }

        public void FinalizeExtension() { }

        public void RegisterInstance(Type type, object instance)
        {
            Instance.UseInstance(type, instance);
        }

        public void RegisterSingleton(Type type)
        {
            Instance.Register(type, Reuse.Singleton);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Instance.Register(from, to, Reuse.Singleton);
        }

        public void RegisterType(Type from, Type to)
        {
            Instance.Register(from, to);
        }

        public void RegisterType(Type from, Type to, string name)
        {
            Instance.Register(from, to, serviceKey: name);
        }

        public void RegisterType(Type type)
        {
            Instance.Register(type);
        }

        public void RegisterType(Type type, string name)
        {
            Instance.Register(type, serviceKey: name);
        }

        public object Resolve(Type type)
        {
            return Instance.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.Resolve(type, serviceKey: name);
        }
    }
}
