using System;
using System.Collections.Generic;
using Moq;
using Prism.Ioc;
using Prism.Navigation;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationContainerMock : IContainerExtension, IDisposable
    {
        Dictionary<string, Type> _registeredPages = new Dictionary<string, Type>();

        public object Instance => throw new NotImplementedException();

        public IScopedProvider CurrentScope { get; private set; }

        public IContainerRegistry Register(string key, Type type)
        {
            if (!_registeredPages.ContainsKey(key))
            {
                _registeredPages.Add(key, type);
                PageNavigationRegistry.Register(key, type);
            }
            return this;
        }

        public object Resolve(Type type)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type, string name)
        {
            if (_registeredPages.ContainsKey(name))
                return Activator.CreateInstance(_registeredPages[name]);

            return null;
        }

        public IContainerRegistry Register(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type from, Type to, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type type)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterType(Type type)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterType(Type type, string name)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            PageNavigationRegistry.ClearRegistrationCache();
        }

        public void FinalizeExtension()
        {

        }

        public bool IsRegistered(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            throw new NotImplementedException();
        }

        public IScopedProvider CreateScope()
        {
            CurrentScope = Mock.Of<IScopedProvider>();
            return CurrentScope;
        }

        public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type type, Func<object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes)
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
    }
}
