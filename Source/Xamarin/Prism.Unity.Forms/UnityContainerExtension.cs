using Prism.Ioc;
using Prism.Mvvm;
using System;
using Unity;
using Unity.Resolution;
using Xamarin.Forms;

namespace Prism.Unity
{
    public class UnityContainerExtension : IContainerExtension<IUnityContainer>
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
            ResolverOverride[] overrides = null;

            switch (view)
            {
                case Page page:
                    overrides = new ResolverOverride[]
                    {
                        new DependencyOverride(
                            typeof(Navigation.INavigationService),
                            this.CreateNavigationService(page)
                        )
                    };
                    break;
                case BindableObject bindable:
                    var attachedPage = bindable.GetValue(ViewModelLocator.AutowirePartialViewProperty) as Page;
                    if (attachedPage != null)
                    {
                        overrides = new ResolverOverride[]
                        {
                            new DependencyOverride(
                                typeof(Navigation.INavigationService),
                                this.CreateNavigationService(attachedPage)
                            )
                        };
                    }
                    break;
            }

            return Instance.Resolve(viewModelType, overrides);
        }
    }
}
