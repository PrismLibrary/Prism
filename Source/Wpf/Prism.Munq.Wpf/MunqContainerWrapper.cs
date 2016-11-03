using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Munq;

namespace Prism.Munq
{
    public class MunqContainerWrapper : IMunqContainer
    {
        private IocContainer wrappedContainer;

        public MunqContainerWrapper()
        {
            wrappedContainer = new IocContainer();
        }

        public ILifetimeManager DefaultLifetimeManager
        {
            get { return wrappedContainer.DefaultLifetimeManager; }
            set { wrappedContainer.DefaultLifetimeManager = value; }
        }

        public bool CanResolve(Type type)
        {
            return wrappedContainer.CanResolve(type);
        }

        public bool CanResolve(string name, Type type)
        {
            return wrappedContainer.CanResolve(name, type);
        }

        public bool CanResolve<TType>() where TType : class
        {
            return wrappedContainer.CanResolve<TType>();
        }

        public bool CanResolve<TType>(string name) where TType : class
        {
            return wrappedContainer.CanResolve<TType>(name);
        }

        public IRegistration GetRegistration(Type type)
        {
            return wrappedContainer.GetRegistration(type);
        }

        public IRegistration GetRegistration(string name, Type type)
        {
            return wrappedContainer.GetRegistration(name, type);
        }

        public IRegistration GetRegistration<TType>() where TType : class
        {
            return wrappedContainer.GetRegistration<TType>();
        }

        public IRegistration GetRegistration<TType>(string name) where TType : class
        {
            return wrappedContainer.GetRegistration<TType>(name);
        }

        public IEnumerable<IRegistration> GetRegistrations(Type type)
        {
            return wrappedContainer.GetRegistrations(type);
        }

        public IEnumerable<IRegistration> GetRegistrations<TType>() where TType : class
        {
            return wrappedContainer.GetRegistrations<TType>();
        }

        public Func<object> LazyResolve(Type type)
        {
            return wrappedContainer.LazyResolve(type);
        }

        public Func<object> LazyResolve(string name, Type type)
        {
            return wrappedContainer.LazyResolve(name, type);
        }

        public Func<TType> LazyResolve<TType>() where TType : class
        {
            return wrappedContainer.LazyResolve<TType>();
        }

        public Func<TType> LazyResolve<TType>(string name) where TType : class
        {
            return wrappedContainer.LazyResolve<TType>(name);
        }

        public IRegistration Register(Type tType, Type tImpl)
        {
            return wrappedContainer.Register(tType, tImpl);
        }

        public IRegistration Register(Type type, Func<IDependencyResolver, object> func)
        {
            return wrappedContainer.Register(type, func);
        }

        public IRegistration Register(string name, Type tType, Type tImpl)
        {
            return wrappedContainer.Register(name, tType, tImpl);
        }

        public IRegistration Register(string name, Type type, Func<IDependencyResolver, object> func)
        {
            return wrappedContainer.Register(name, type, func);
        }

        public IRegistration Register<TType>(Func<IDependencyResolver, TType> func) where TType : class
        {
            return wrappedContainer.Register<TType>(func);
        }

        public IRegistration Register<TType>(string name, Func<IDependencyResolver, TType> func) where TType : class
        {
            return wrappedContainer.Register<TType>(name, func);
        }

        public IRegistration RegisterInstance(Type type, object instance)
        {
            return wrappedContainer.RegisterInstance(type, instance);
        }

        public IRegistration RegisterInstance(string name, Type type, object instance)
        {
            return wrappedContainer.RegisterInstance(name, type, instance);
        }

        public IRegistration RegisterInstance<TType>(TType instance) where TType : class
        {
            return wrappedContainer.RegisterInstance<TType>(instance);
        }

        public IRegistration RegisterInstance<TType>(string name, TType instance) where TType : class
        {
            return wrappedContainer.RegisterInstance<TType>(name, instance);
        }

        public void Remove(IRegistration ireg)
        {
            wrappedContainer.Remove(ireg);
        }

        public object Resolve(Type type)
        {
            return wrappedContainer.Resolve(type);
        }

        public object Resolve(string name, Type type)
        {
            return wrappedContainer.Resolve(name, type);
        }

        public TType Resolve<TType>() where TType : class
        {
            return wrappedContainer.Resolve<TType>();
        }

        public TType Resolve<TType>(string name) where TType : class
        {
            return wrappedContainer.Resolve<TType>(name);
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            return wrappedContainer.ResolveAll(type);
        }

        public IEnumerable<TType> ResolveAll<TType>() where TType : class
        {
            return wrappedContainer.ResolveAll<TType>();
        }

        IRegistration IDependecyRegistrar.Register<TType, TImpl>()
        {
            return wrappedContainer.Register<TType, TImpl>();
        }

        IRegistration IDependecyRegistrar.Register<TType, TImpl>(string name)
        {
            return wrappedContainer.Register<TType, TImpl>(name);
        }
    }
}
