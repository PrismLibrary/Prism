

using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace Prism.Wpf.Tests.Mocks
{
    internal class MockContainerAdapter : ServiceLocatorImplBase
    {
        public Dictionary<Type, object> ResolvedInstances = new Dictionary<Type, object>();

        protected override object DoGetInstance(Type serviceType, string key)
        {
            object resolvedInstance;
            if (!this.ResolvedInstances.ContainsKey(serviceType))
            {
                resolvedInstance = Activator.CreateInstance(serviceType);
                this.ResolvedInstances.Add(serviceType, resolvedInstance);
            }
            else
            {
                resolvedInstance = this.ResolvedInstances[serviceType];
            }

            return resolvedInstance;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            throw new System.NotImplementedException();
        }
    }
}