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

        public bool SupportsModules => true;

        public object GetInstance(string key)
        {
            if (_registeredPages.ContainsKey(key))
                return Activator.CreateInstance(_registeredPages[key]);

            return null;
        }

        public void Register(string key, Type type)
        {
            if (!_registeredPages.ContainsKey(key))
            {
                _registeredPages.Add(key, type);
                PageNavigationRegistry.Register(key, type);
            }
        }

        public object Resolve(Type type)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type, string name)
        {
            throw new NotImplementedException();
        }

        public void Register(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public void Register(Type from, Type to, string name)
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance(Type type, object instance)
        {
            throw new NotImplementedException();
        }

        public void RegisterSingleton(Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterSingleton(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public void RegisterType(Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterType(Type type, string name)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            PageNavigationRegistryFixture.ResetPageNavigationRegistry();
        }

        public void FinalizeExtension()
        {
            
        }

        public object ResolveViewModelForView(object view, Type viewModelType)
        {
            throw new NotImplementedException();
        }
    }
}
