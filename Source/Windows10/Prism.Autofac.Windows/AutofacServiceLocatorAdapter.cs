using Autofac;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Autofac.Windows
{
    /// <summary>
    /// Defines a <see cref="IContainer"/> adapter for the <see cref="IServiceLocator"/> interface to be used by the Prism Library.
    /// </summary>
    public class AutofacServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of <see cref="AutofacServiceLocatorAdapter"/>.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> that will be used
        /// by the <see cref="DoGetInstance"/> and <see cref="DoGetAllInstances"/> methods.</param>
        public AutofacServiceLocatorAdapter(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
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
            return key != null ?
                _container.ResolveNamed(key, serviceType) :
                _container.Resolve(serviceType);
        }

        /// <summary>
        /// Resolves all the instances of the requested service.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>Sequence of service instance objects.</returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);

            object instance = _container.Resolve(enumerableType);
            return ((IEnumerable)instance).Cast<object>();
        }
    }
}
