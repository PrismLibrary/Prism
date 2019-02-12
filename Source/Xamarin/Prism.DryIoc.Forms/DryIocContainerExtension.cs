using System;
using System.Linq;
using DryIoc;
using Prism.Ioc;

namespace Prism.DryIoc
{
    public class DryIocContainerExtension : IContainerExtension<IContainer>
    {
        public IContainer Instance { get; }

        public DryIocContainerExtension(IContainer container)
        {
            Instance = container;
        }

        public void FinalizeExtension() { }

        public void RegisterInstance(Type type, object instance)
        {
            Instance.UseInstance(type, instance);
        }

        public void RegisterInstance(Type type, object instance, string name)
        {
            Instance.UseInstance(type, instance, serviceKey: name);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Instance.Register(from, to, Reuse.Singleton);
        }

        public void RegisterSingleton(Type from, Type to, string name)
        {
            Instance.Register(from, to, Reuse.Singleton, serviceKey: name);
        }

        public void Register(Type from, Type to)
        {
            Instance.Register(from, to);
        }

        public void Register(Type from, Type to, string name)
        {
            Instance.Register(from, to, serviceKey: name);
        }

        public object Resolve(Type type)
        {
            return Instance.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.Resolve(type, serviceKey: name);
        }

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            return Instance.Resolve(type, args: parameters.Select(p => p.Instance).ToArray());
        }

        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            return Instance.Resolve(type, name, args: parameters.Select(p => p.Instance).ToArray());
        }

        public bool IsRegistered(Type type)
        {
            return Instance.IsRegistered(type);
        }

        public bool IsRegistered(Type type, string name)
        {
            return Instance.IsRegistered(type, name);
        }
    }
}
