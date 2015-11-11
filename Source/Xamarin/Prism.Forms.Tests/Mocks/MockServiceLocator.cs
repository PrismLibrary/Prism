using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;

namespace Prism.Forms.Tests.Mocks
{
    internal class MockServiceLocator : ServiceLocatorImplBase
    {
        public Func<object> ResolveMethod;

        public MockServiceLocator(Func<object> resolveMethod)
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
}
