using Prism.Ioc;
using System;
using Unity;

namespace Prism.Unity
{
    public class UnityContainerExtension : IContainerExtension<IUnityContainer>
    {
        private readonly IUnityContainer _container;

        public IUnityContainer Instance => _container;

        public bool SupportsModules => true;

        public UnityContainerExtension(IUnityContainer container) => _container = container;

        public void RegisterInstance(Type type, object instance)
        {
            _container.RegisterInstance(type, instance);
        }

        public void RegisterSingleton(Type type)
        {
            _container.RegisterSingleton(type);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            _container.RegisterSingleton(from, to);
        }

        public void RegisterType(Type type)
        {
            _container.RegisterType(type);
        }

        public void RegisterType(Type type, string name)
        {
            _container.RegisterType(type, name);
        }

        public void RegisterType(Type from, Type to)
        {
            _container.RegisterType(from, to);
        }

        public void RegisterType(Type from, Type to, string name)
        {
            _container.RegisterType(from, to, name);
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return _container.Resolve(type, name);
        }
    }
}
