using Prism.Forms.Tests.Navigation;
using Prism.Ioc;
using Prism.Navigation;
using System;
using System.Collections.Generic;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationContainerMock : IContainerExtension, IDisposable
    {
        Dictionary<string, Type> _registeredPages = new Dictionary<string, Type>();

        public object Instance => throw new NotImplementedException();

        public object GetInstance(string key)
        {
            if (_registeredPages.ContainsKey(key))
                return Activator.CreateInstance(_registeredPages[key]);

            return null;
        }

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
            throw new NotImplementedException();
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
    }
}
