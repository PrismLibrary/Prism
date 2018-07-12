using Ninject;
using Prism.Ioc;
using System;

namespace Prism.Ninject.Ioc
{
    public class NinjectContainerExtension : IContainerExtension<IKernel>
    {
        public IKernel Instance { get; }

        public bool SupportsModules => true;

        public NinjectContainerExtension()
            : this(new StandardKernel()) { }

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
            Instance.Bind(from).To(to).InSingletonScope();
        }

        public void Register(Type from, Type to)
        {
            Instance.Bind(from).To(to).InTransientScope();
        }

        public void Register(Type from, Type to, string name)
        {
            Instance.Bind(from).To(to).InTransientScope().Named(name);
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
            return Instance.Get(viewModelType);
        }
    }
}
