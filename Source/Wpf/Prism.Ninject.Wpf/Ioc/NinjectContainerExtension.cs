using System;
using System.Linq;
using Ninject;
using Ninject.Parameters;
using Prism.Ioc;

namespace Prism.Ninject.Ioc
{
    public class NinjectContainerExtension : IContainerExtension<IKernel>
    {
        public IKernel Instance { get; }

        public NinjectContainerExtension()
            : this(new StandardKernel()) { }

        public NinjectContainerExtension(IKernel kernel)
        {
            Instance = kernel;
        }

        public void FinalizeExtension() { }

        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            Instance.Bind(type).ToConstant(instance);
            return this;
        }

        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            Instance.Bind(type).ToConstant(instance).Named(name);
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            Instance.Bind(from).To(to).InSingletonScope();
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            Instance.Bind(from).To(to).InSingletonScope().Named(name);
            return this;
        }

        public IContainerRegistry Register(Type from, Type to)
        {
            Instance.Bind(from).To(to).InTransientScope();
            return this;
        }

        public IContainerRegistry Register(Type from, Type to, string name)
        {
            Instance.Bind(from).To(to).InTransientScope().Named(name);
            return this;
        }

        public object Resolve(Type type)
        {
            return Instance.Get(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.Get(type, name);
        }

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            var overrides = parameters.Select(p => new TypeMatchingConstructorArgument(p.Type, (c,t) => p.Instance)).ToArray();
            return Instance.Get(type, overrides);
        }

        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            var overrides = parameters.Select(p => new TypeMatchingConstructorArgument(p.Type, (c, t) => p.Instance)).ToArray();
            return Instance.Get(type, name, overrides);
        }

        public bool IsRegistered(Type type)
        {
            return IsRegistered(type);
        }

        public bool IsRegistered(Type type, string name)
        {
            return Instance.IsRegistered(type);
        }
    }
}
