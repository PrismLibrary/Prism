using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Ioc;
using Prism.Navigation;
using Unity;
using Unity.Injection;
using Unity.Resolution;
using Windows.UI.Xaml.Controls;

namespace Prism.Unity
{
    public sealed class UnityContainerExtension : IContainerExtension<IUnityContainer>
    {
        public IUnityContainer Instance { get; }

        public bool SupportsModules => true;

        public UnityContainerExtension(IUnityContainer container) => Instance = container;

        public void FinalizeExtension() { }

        public void RegisterInstance(Type type, object instance)
        {
            Instance.RegisterInstance(type, instance);
        }

        public void RegisterInstance(Type type, object instance, string name)
        {
            Instance.RegisterInstance(type, name, instance);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Instance.RegisterSingleton(from, to);
        }

        public void RegisterSingleton(Type from, Type to, string name)
        {
            Instance.RegisterSingleton(from, to, name);
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
            if (view is Page page && page.Frame != null)
            {
                var service = NavigationService.Instances[page.Frame];
                ResolverOverride[] overrides = null;

                overrides = new ResolverOverride[]
                {
                    new DependencyOverride(
                        typeof(INavigationService),
                        service
                    )
                };
                return Instance.Resolve(viewModelType, overrides);
            }
            else
            {
                return Instance.Resolve(viewModelType);
            }
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
