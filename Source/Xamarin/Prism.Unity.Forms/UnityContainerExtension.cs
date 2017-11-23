using Prism.Ioc;
using System;
using Unity;

namespace Prism.Unity
{
    public class UnityContainerExtension : IContainerExtension<IUnityContainer>
    {
        private readonly IUnityContainer _container;

        public IUnityContainer Instance => _container;

        public UnityContainerExtension(IUnityContainer container) => _container = container;

        public void RegisterInstance<TInterface>(TInterface instance)
        {
            _container.RegisterInstance<TInterface>(instance);
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
        {
            _container.RegisterSingleton<TFrom, TTo>();
        }

        public void RegisterSingleton<T>()
        {
            _container.RegisterSingleton<T>();
        }

        public void RegisterType(Type from, Type to)
        {
            _container.RegisterType(from, to);
        }

        public void RegisterType(Type from, Type to, string name)
        {
            _container.RegisterType(from, to, name);
        }

        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            _container.RegisterType<TFrom, TTo>();
        }

        public void RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            _container.RegisterType<TFrom, TTo>(name);
        }

        public void RegisterType<T>()
        {
            _container.RegisterType<T>();
        }

        public void RegisterType<T>(string name)
        {
            _container.RegisterType<T>(name);
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return _container.Resolve(type, name);
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            return _container.Resolve<T>(name);
        }
    }
}
