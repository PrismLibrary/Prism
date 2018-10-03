using System;
using System.Linq;
using Prism.Ioc;
using Prism.Navigation;
using Unity;
using Unity.Resolution;
using Windows.UI.Xaml.Controls;

namespace Prism.Unity
{
    public sealed class UnityContainerExtension : IContainerExtension<IUnityContainer>, IDependencyResolver
    {
        public IUnityContainer Instance { get; }

        public bool SupportsModules => true;

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
            if (view is Page page)
            {
                var service = NavigationServiceLocator.GetNavigationService(page);
                return Resolve(viewModelType, (typeof(INavigationService), service));
            }
            else
            {
                return Resolve(viewModelType);
            }
        }

        public object Resolve(Type serviceType, params (Type resolvingType, object instance)[] args)
        {
            return Instance.Resolve(serviceType,
                args.Select(a => new DependencyOverride(a.resolvingType, a.instance)).ToArray());
        }
    }
}
