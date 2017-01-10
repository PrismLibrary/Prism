using System;
using System.Collections.Generic;
using Munq;

namespace Prism.Munq
{
    public class MunqContainerWrapper : IMunqContainer
    {
        private IocContainer _wrappedContainer;

        public MunqContainerWrapper()
        {
            _wrappedContainer = new IocContainer();
        }

        public ILifetimeManager DefaultLifetimeManager
        {
            get { return _wrappedContainer.DefaultLifetimeManager; }
            set { _wrappedContainer.DefaultLifetimeManager = value; }
        }

        public bool CanResolve(Type type)
        {
            return _wrappedContainer.CanResolve(type);
        }

        public bool CanResolve(string name, Type type)
        {
            return _wrappedContainer.CanResolve(name, type);
        }

        public bool CanResolve<TType>() where TType : class
        {
            return _wrappedContainer.CanResolve<TType>();
        }

        public bool CanResolve<TType>(string name) where TType : class
        {
            return _wrappedContainer.CanResolve<TType>(name);
        }

        public IRegistration GetRegistration(Type type)
        {
            return _wrappedContainer.GetRegistration(type);
        }

        public IRegistration GetRegistration(string name, Type type)
        {
            return _wrappedContainer.GetRegistration(name, type);
        }

        public IRegistration GetRegistration<TType>() where TType : class
        {
            return _wrappedContainer.GetRegistration<TType>();
        }

        public IRegistration GetRegistration<TType>(string name) where TType : class
        {
            return _wrappedContainer.GetRegistration<TType>(name);
        }

        public IEnumerable<IRegistration> GetRegistrations(Type type)
        {
            return _wrappedContainer.GetRegistrations(type);
        }

        public IEnumerable<IRegistration> GetRegistrations<TType>() where TType : class
        {
            return _wrappedContainer.GetRegistrations<TType>();
        }

        public Func<object> LazyResolve(Type type)
        {
            return _wrappedContainer.LazyResolve(type);
        }

        public Func<object> LazyResolve(string name, Type type)
        {
            return _wrappedContainer.LazyResolve(name, type);
        }

        public Func<TType> LazyResolve<TType>() where TType : class
        {
            return _wrappedContainer.LazyResolve<TType>();
        }

        public Func<TType> LazyResolve<TType>(string name) where TType : class
        {
            return _wrappedContainer.LazyResolve<TType>(name);
        }

        public IRegistration Register(Type tType, Type tImpl)
        {
            return _wrappedContainer.Register(tType, tImpl);
        }

        public IRegistration Register(Type type, Func<IDependencyResolver, object> func)
        {
            return _wrappedContainer.Register(type, func);
        }

        public IRegistration Register(string name, Type tType, Type tImpl)
        {
            return _wrappedContainer.Register(name, tType, tImpl);
        }

        public IRegistration Register(string name, Type type, Func<IDependencyResolver, object> func)
        {
            return _wrappedContainer.Register(name, type, func);
        }

        public IRegistration Register<TType>(Func<IDependencyResolver, TType> func) where TType : class
        {
            return _wrappedContainer.Register<TType>(func);
        }

        public IRegistration Register<TType>(string name, Func<IDependencyResolver, TType> func) where TType : class
        {
            return _wrappedContainer.Register<TType>(name, func);
        }

        public IRegistration RegisterInstance(Type type, object instance)
        {
            return _wrappedContainer.RegisterInstance(type, instance);
        }

        public IRegistration RegisterInstance(string name, Type type, object instance)
        {
            return _wrappedContainer.RegisterInstance(name, type, instance);
        }

        public IRegistration RegisterInstance<TType>(TType instance) where TType : class
        {
            return _wrappedContainer.RegisterInstance<TType>(instance);
        }

        public IRegistration RegisterInstance<TType>(string name, TType instance) where TType : class
        {
            return _wrappedContainer.RegisterInstance<TType>(name, instance);
        }

        public void Remove(IRegistration ireg)
        {
            _wrappedContainer.Remove(ireg);
        }

        public object Resolve(Type type)
        {
            return _wrappedContainer.Resolve(type);
        }

        public object Resolve(string name, Type type)
        {
            return _wrappedContainer.Resolve(name, type);
        }

        public TType Resolve<TType>() where TType : class
        {
            return _wrappedContainer.Resolve<TType>();
        }

        public TType Resolve<TType>(string name) where TType : class
        {
            return _wrappedContainer.Resolve<TType>(name);
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            return _wrappedContainer.ResolveAll(type);
        }

        public IEnumerable<TType> ResolveAll<TType>() where TType : class
        {
            return _wrappedContainer.ResolveAll<TType>();
        }

        IRegistration IDependecyRegistrar.Register<TType, TImpl>()
        {
            return _wrappedContainer.Register<TType, TImpl>();
        }

        IRegistration IDependecyRegistrar.Register<TType, TImpl>(string name)
        {
            return _wrappedContainer.Register<TType, TImpl>(name);
        }
    }
}
