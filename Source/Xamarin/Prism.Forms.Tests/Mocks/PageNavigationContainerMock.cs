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

        public void Register(string key, Type type)
        {
            if (!_registeredPages.ContainsKey(key))
            {
                _registeredPages.Add(key, type);
                PageNavigationRegistry.Register(key, type);
            }
        }

        public void Dispose()
        {
            PageNavigationRegistryFixture.ResetPageNavigationRegistry();
        }

        public object Resolve(Type type)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type, string name)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>()
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>(string name)
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance<TInterface>(TInterface instance)
        {
            throw new NotImplementedException();
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
        {
            throw new NotImplementedException();
        }

        public void RegisterType(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public void RegisterType(Type from, Type to, string name)
        {
            throw new NotImplementedException();
        }

        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            throw new NotImplementedException();
        }

        public void RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            throw new NotImplementedException();
        }

        public void RegisterSingleton<T>()
        {
            throw new NotImplementedException();
        }

        public void RegisterType<T>()
        {
            throw new NotImplementedException();
        }

        public void RegisterType<T>(string name)
        {
            throw new NotImplementedException();
        }
    }
}
