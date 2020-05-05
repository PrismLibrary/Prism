using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Ioc;

namespace Prism.Forms.Tests.Services.Mocks.Ioc
{
    public class DialogContainerExtension : IContainerExtension
    {
        private readonly MockContainer _container = new MockContainer();

        public IScopedProvider CurrentScope { get; }

        public void CreateScope()
        {
            throw new NotImplementedException();
        }

        public void FinalizeExtension()
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type from, Type to)
        {
            _container.Add(new TransientRegistration
            {
                ServiceType = from,
                ImplementingType = to
            });
            return this;
        }

        public IContainerRegistry Register(Type from, Type to, string name)
        {
            _container.Add(new TransientRegistration
            {
                ServiceType = from,
                ImplementingType = to,
                Name = name
            });
            return this;
        }

        public IContainerRegistry Register(Type type, Func<object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterScoped(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type)
        {
            return _container.GetRegistration(type).Create();
        }

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type, string name)
        {
            return _container.GetRegistration(type, name).Create();
        }

        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            throw new NotImplementedException();
        }

        IScopedProvider IContainerProvider.CreateScope()
        {
            throw new NotImplementedException();
        }

        private class MockContainer
        {
            private readonly IList<IRegistration> _registrations = new List<IRegistration>();

            public void Add(IRegistration registration) => _registrations.Add(registration);

            public IRegistration GetRegistration(Type type) =>
                _registrations.FirstOrDefault(x => x.ServiceType == type && string.IsNullOrEmpty(x.Name));

            public IRegistration GetRegistration(Type type, string name) =>
                _registrations.FirstOrDefault(x => x.ServiceType == type && x.Name == name);
        }
    }
}
