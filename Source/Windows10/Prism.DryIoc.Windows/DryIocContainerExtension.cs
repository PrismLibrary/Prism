using System;
using DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using Windows.UI.Xaml.Controls;

namespace Prism.DryIoc
{
    public sealed class DryIocContainerExtension : IContainerExtension<IContainer>
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

        public void RegisterSingleton(Type from, Type to)
        {
            Instance.Register(from, to, Reuse.Singleton);
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

        public object ResolveViewModelForView(object view, Type viewModelType)
        {
            if (view is Page page && page.Frame != null)
            {
                var service = NavigationService.Instances[page.Frame];
                return Instance.Resolve(viewModelType, new[] { service });
            }
            return Instance.Resolve(viewModelType);
        }
    }
}
