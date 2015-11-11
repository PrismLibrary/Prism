using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;

namespace Prism.Forms.Tests.Mocks
{
    internal class ServiceLocatorMock : ServiceLocatorImplBase
    {
        public Func<object> ResolveMethod;

        public ServiceLocatorMock(Func<object> resolveMethod)
        {
            ResolveMethod = resolveMethod;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            object instance = ResolveMethod();

            // If the instance was not found, throw exception.
            if (instance.GetType() != serviceType)
            {
                throw new ActivationException("Type not found.");
            }
            else
            {
                return instance;
            }
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return null;
        }
    }

    internal class PageNavigationServiceLocatorMock : ServiceLocatorImplBase
    {
        Dictionary<string, Type> _registeredPages = new Dictionary<string, Type>();

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (_registeredPages.ContainsKey(key))
                return Activator.CreateInstance(_registeredPages[key]);

            return null;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return null;
        }

        public void Register(string key, Type type)
        {
            if (!_registeredPages.ContainsKey(key))
                _registeredPages.Add(key, type);
        }
    }
}
