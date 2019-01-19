using Ninject;
using Ninject.Parameters;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void RegisterInstance(Type type, object instance, string name)
        {
            Instance.Bind(type).ToConstant(instance).Named(name);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Instance.Bind(from).To(to).InSingletonScope();
        }

        public void RegisterSingleton(Type from, Type to, string name)
        {
            Instance.Bind(from).To(to).InSingletonScope().Named(name);
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

        public object Resolve(Type type, IDictionary<Type, object> parameters)
        {
            var overrides = parameters.Select(p => new TypeMatchingConstructorArgument(p.Key, (c,t) => p.Value)).ToArray();
            return Instance.Get(type, overrides);
        }

        public void RegisterMany(Type implementingType)
        {
            Instance.Bind(implementingType.GetInterfaces().ToArray()).To(implementingType).InSingletonScope();
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
