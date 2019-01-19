using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.DryIoc
{
    public class DryIocContainerExtension : IContainerExtension<IContainer>
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

        public virtual object ResolveViewModelForView(object view, Type viewModelType)
        {
            switch (view)
            {
                case Page page:
                    var getVM = Instance.Resolve<Func<Page, object>>(viewModelType);
                    return getVM(page);
                case BindableObject bindable:
                    if (bindable.GetValue(ViewModelLocator.AutowirePartialViewProperty) is Page attachedPage)
                    {
                        var getVMForPartial = Instance.Resolve<Func<Page, object>>(viewModelType);
                        return getVMForPartial(attachedPage);
                    }
                    break;
            }

            return Instance.Resolve(viewModelType);
        }

        public object Resolve(Type type, IDictionary<Type, object> parameters)
        {
            return Instance.Resolve(type, args: parameters.Select(p => p.Value).ToArray());
        }

        public void RegisterMany(Type implementingType)
        {
            Instance.RegisterMany(new Type[] { implementingType }, Reuse.Singleton, serviceTypeCondition: t => implementingType.ImplementsServiceType(t));
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
