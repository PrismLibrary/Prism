using Ninject;
using Ninject.Parameters;
using Prism.Ioc;
using System;
using System.Linq;
using Xamarin.Forms;

namespace Prism.Ninject
{
    public class NinjectContainerExtension : IContainerExtension<IKernel>
    {
        public IKernel Instance { get; }

        public bool SupportsModules => true;

        public NinjectContainerExtension(IKernel kernel)
        {
            Instance = kernel;
        }

        public void FinalizeExtension() { }

        public void RegisterInstance(Type type, object instance)
        {
            Instance.Rebind(type).ToConstant(instance);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Instance.Rebind(from).To(to).InSingletonScope();
        }

        public void Register(Type from, Type to)
        {
            Instance.Rebind(from).To(to).InTransientScope();
        }

        public void Register(Type from, Type to, string name)
        {
            if(Instance.GetBindings(from).Any(b => b.Metadata.Name == name))
            {
                Instance.Rebind(from).To(to).InTransientScope().Named(name);
            }
            else
            {
                Instance.Bind(from).To(to).InTransientScope().Named(name);
            }
        }

        public object Resolve(Type type)
        {
            return Instance.Get(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.Get(type, name);
        }

        public object ResolveViewModelForView(object view, Type viewModelType)
        {
            IParameter[] overrides = null;

            if (view is Page page)
            {
                overrides = new IParameter[]
                {
                    new ConstructorArgument(PrismApplicationBase.NavigationServiceParameterName, this.CreateNavigationService(page))
                };
            }

            return Instance.Get(viewModelType, overrides);
        }
    }
}
