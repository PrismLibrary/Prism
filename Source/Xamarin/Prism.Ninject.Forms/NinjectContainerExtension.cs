using Ninject;
using Ninject.Parameters;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;
using System;
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
            Instance.Bind(type).ToConstant(instance);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Instance.Bind(to).To(from).InSingletonScope();
        }

        public void RegisterType(Type from, Type to)
        {
            Instance.Bind(to).To(from).InTransientScope();
        }

        public void RegisterType(Type from, Type to, string name)
        {
            Instance.Bind(to).To(from).InTransientScope().Named(name);
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
