using Prism.Ioc;
using DryIoc;
using System;

namespace Prism.DryIoc
{
    public class DryIocContainerExtension : IContainerExtension<IContainer>, IContainerSupportsModularization
    {
        public DryIocContainerExtension(IContainer container)
        {
            Instance = container;
        }

        public IContainer Instance { get; }

        public void RegisterInstance<TInterface>(TInterface instance) 
            where TInterface : class
        {
            Instance.UseInstance(instance);
        }

        public void RegisterSingleton<T>()
        {
            Instance.Register<T>(Reuse.Singleton);
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
        {
            Instance.Register<TFrom, TTo>(Reuse.Singleton);
        }

        public void RegisterType(Type from, Type to)
        {
            Instance.Register(from, to);
        }

        public void RegisterType(Type from, Type to, string name)
        {
            Instance.Register(from, to, serviceKey: name);
        }

        public void RegisterType<T>()
        {
            Instance.Register<T>();
        }

        public void RegisterType<T>(string name)
        {
            Instance.Register<T>(serviceKey: name);
        }

        public void RegisterType<TFrom, TTo>() 
            where TTo : TFrom
        {
            Instance.Register<TFrom, TTo>();
        }

        public void RegisterType<TFrom, TTo>(string name) 
            where TTo : TFrom
        {
            Instance.Register<TFrom, TTo>(serviceKey: name);
        }

        public object Resolve(Type type)
        {
            return Instance.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.Resolve(type, serviceKey: name);
        }

        public T Resolve<T>()
        {
            return Instance.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            return Instance.Resolve<T>(name);
        }
    }
}
