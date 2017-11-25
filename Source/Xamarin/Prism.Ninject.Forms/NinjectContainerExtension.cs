using System;
using System.Collections.Generic;
using System.Text;
using Ninject;
using Prism.Ioc;

namespace Prism.Ninject
{
    public class NinjectContainerExtension : IContainerExtension<IKernel>
    {
        public NinjectContainerExtension(IKernel kernel)
        {
            Instance = kernel;
        }

        public IKernel Instance { get; }

        public bool SupportsModules => true;

        public void RegisterInstance(Type type, object instance)
        {
            Instance.Bind(type).ToConstant(instance);
        }

        public void RegisterSingleton(Type type)
        {
            Instance.Bind(type).To(type).InSingletonScope();
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Instance.Bind(to).To(from).InSingletonScope();
        }

        public void RegisterType(Type type)
        {
            Instance.Bind(type).To(type).InTransientScope();
        }

        public void RegisterType(Type type, string name)
        {
            Instance.Bind(type).To(type).Named(name);
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
    }
}
