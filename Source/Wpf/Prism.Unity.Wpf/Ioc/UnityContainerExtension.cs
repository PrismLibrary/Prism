using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Ioc;
using Unity;
using Unity.Injection;
using Unity.Resolution;

namespace Prism.Unity.Ioc
{
    public class UnityContainerExtension : IContainerExtension<IUnityContainer>
    {
        public IUnityContainer Instance { get; }

        public bool SupportsModules => true;

        public UnityContainerExtension() : this(new UnityContainer()) { }

        public UnityContainerExtension(IUnityContainer container) => Instance = container;

        public void FinalizeExtension() { }

        public void RegisterInstance(Type type, object instance)
        {
            Instance.RegisterInstance(type, instance);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Instance.RegisterSingleton(from, to);
        }

        public void Register(Type from, Type to)
        {
            Instance.RegisterType(from, to);
        }

        public void Register(Type from, Type to, string name)
        {
            Instance.RegisterType(from, to, name);
        }

        public object Resolve(Type type)
        {
            return Instance.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.Resolve(type, name);
        }

        public object ResolveViewModelForView(object view, Type viewModelType)
        {
            return Instance.Resolve(viewModelType);
        }

        public object Resolve(Type type, IDictionary<Type, object> parameters)
        {
            var overrides = parameters.Select(p => new DependencyOverride(p.Key, p.Value)).ToArray();
            return Instance.Resolve(type, overrides);
        }

        public void RegisterMany(Type implementingType)
        {
            Instance.RegisterSingleton(implementingType);
            foreach (var serviceType in implementingType.GetInterfaces())
            {
                Instance.RegisterType(serviceType, new InjectionFactory(x => x.Resolve(implementingType)));
            }
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
