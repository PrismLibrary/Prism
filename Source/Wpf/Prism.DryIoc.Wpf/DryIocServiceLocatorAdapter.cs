using DryIoc;
using CommonServiceLocator;
using System;
using System.Collections.Generic;

namespace Prism.DryIoc
{
    public class DryIocServiceLocatorAdapter : ServiceLocatorImplBase
    {
        /// <summary>Exposes underlying Container for direct operation.</summary>
        public readonly IContainer _container;

        /// <summary>Creates new locator as adapter for provided container.</summary>
        /// <param name="container">Container to use/adapt.</param>
        public DryIocServiceLocatorAdapter(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _container = container;
        }

        /// <summary>Resolves service from container. Throws if unable to resolve.</summary>
        /// <param name="serviceType">Service type to resolve.</param>
        /// <param name="key">(optional) Service key to resolve.</param>
        /// <returns>Resolved service object.</returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            return _container.Resolve(serviceType, key);
        }

        /// <summary>Returns enumerable which when enumerated! resolves all default and named 
        /// implementations/registrations of requested service type. 
        /// If no services resolved when enumerable accessed, no exception is thrown - enumerable is empty.</summary>
        /// <param name="serviceType">Service type to resolve.</param>
        /// <returns>Returns enumerable which will return resolved service objects.</returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            return _container.ResolveMany<object>(serviceType);
        }
    }
}
