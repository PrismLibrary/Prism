using Prism.Forms.Tests.Navigation;
using Prism.Navigation;
using System;
using System.Collections.Generic;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationContainerMock : IDisposable
    {
        Dictionary<string, Type> _registeredPages = new Dictionary<string, Type>();

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
    }
}
