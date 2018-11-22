using System;
using Prism.Ioc;
using Prism.Navigation;
using Unity;
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
    }
}
