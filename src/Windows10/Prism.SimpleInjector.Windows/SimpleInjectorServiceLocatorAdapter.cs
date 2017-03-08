using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using SimpleInjector;

namespace Prism.SimpleInjector.Windows
{
    /// <summary>
    /// Defines a <see cref="Container" /> adapter for the <see cref="IServiceLocator" /> interface to be used by the Prism Library.
    /// </summary>
    public class SimpleInjectorServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly Container _container;

        /// <summary>
        /// Initializes a new instance of <see cref="SimpleInjectorServiceLocatorAdapter" />.
        /// </summary>
        /// <param name="container">
        /// The <see cref="Container" /> that will be used
        /// by the <see cref="DoGetInstance" /> and <see cref="DoGetAllInstances" /> methods.
        /// </param>
        public SimpleInjectorServiceLocatorAdapter(Container container)
        {
            _container = container;
        }

        /// <summary>
        /// Resolves the instance of the requested service.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>The requested service instance.</returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key != null)
            {
                throw new NotSupportedException();
            }

            return _container.GetInstance(serviceType);
        }

        /// <summary>
        /// Resolves all the instances of the requested service.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>Sequence of service instance objects.</returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }
    }
}