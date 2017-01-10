using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Munq;

namespace Prism.Munq
{
    /// <summary>
    /// Defines a <see cref="IDependencyResolver"/> adapter for the <see cref="IServiceLocator"/> interface to be used by the Prism Library.
    /// </summary>
    public class MunqServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly IDependencyResolver _munqContainer;

        /// <summary>
        /// Initializes a new instance of <see cref="MunqServiceLocatorAdapter"/>.
        /// </summary>
        /// <param name="munqContainer">The <see cref="IUnityContainer"/> that will be used
        /// by the <see cref="DoGetInstance"/> and <see cref="DoGetAllInstances"/> methods.</param>
        public MunqServiceLocatorAdapter(IDependencyResolver munqContainer)
        {
            _munqContainer = munqContainer;
        }

        /// <summary>
        /// Resolves the instance of the requested service.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>The requested service instance.</returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return _munqContainer.Resolve(key, serviceType);
        }

        /// <summary>
        /// Resolves all the instances of the requested service.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>Sequence of service instance objects.</returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _munqContainer.ResolveAll(serviceType);
        }
    }
}