using System;
using System.Collections.Generic;
using LightInject;
using Microsoft.Practices.ServiceLocation;

namespace Prism.LightInject.Windows
{
    /// <summary>
    /// An <see cref="IServiceLocator" /> adapter for the LightInject service container.
    /// </summary>
    public class LightInjectServiceLocatorAdaptor : ServiceLocatorImplBase
    {
        private readonly IServiceContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightInjectServiceLocatorAdaptor" /> class.
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer" /> instance wrapped by this class.</param>
        internal LightInjectServiceLocatorAdaptor(IServiceContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Gets a named instance of the given <paramref name="serviceType" />.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="key">The key of the requested service.</param>
        /// <returns>The requested service instance.</returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key != null)
            {
                return _container.GetInstance(serviceType, key);
            }

            return _container.GetInstance(serviceType);
        }

        /// <summary>
        /// Gets all instances of the given <paramref name="serviceType" />.
        /// </summary>
        /// <param name="serviceType">The type of services to resolve.</param>
        /// <returns>A list that contains all implementations of the <paramref name="serviceType" />.</returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }
    }
}