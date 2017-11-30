using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Grace.DependencyInjection;

namespace Prism.Grace.Windows
{
    /// <summary>
    /// Defines a <see cref="DependencyInjectionContainer"/> adapter for the <see cref="IServiceLocator"/> interface to be used by the Prism Library.
    /// </summary>
    public class GraceServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly DependencyInjectionContainer _container;

        /// <summary>
        /// Initializes a new instance of <see cref="GraceServiceLocatorAdapter"/>.
        /// </summary>
        /// <param name="graceContainer">The <see cref="DependencyInjectionContainer"/> that will be used
        /// by the <see cref="DoGetInstance"/> and <see cref="DoGetAllInstances"/> methods.</param>
        public GraceServiceLocatorAdapter(DependencyInjectionContainer graceContainer)
        {
            _container = graceContainer;
        }

        /// <summary>
        /// Resolves the instance of the requested service.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>The requested service instance.</returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return _container.Locate(serviceType, key);
        }

        /// <summary>
        /// Resolves all the instances of the requested service.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>Sequence of service instance objects.</returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.LocateAll(serviceType);
        }
    }
}