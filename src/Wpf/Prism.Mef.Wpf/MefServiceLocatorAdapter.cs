

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace Prism.Mef
{
    /// <summary>
    /// Provides service location utilizing the Managed Extensibility Framework container.
    /// </summary>
    public class MefServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly CompositionContainer compositionContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefServiceLocatorAdapter"/> class.
        /// </summary>
        /// <param name="compositionContainer">The MEF composition container.</param>
        public MefServiceLocatorAdapter(CompositionContainer compositionContainer)
        {
            this.compositionContainer = compositionContainer;
        }

        /// <summary>
        /// Resolves the instance of the requested service.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <returns>The requested service instance.</returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            List<object> instances = new List<object>();

            IEnumerable<Lazy<object, object>> exports = this.compositionContainer.GetExports(serviceType, null, null);
            if (exports != null)
            {
                instances.AddRange(exports.Select(export => export.Value));
            }

            return instances;
        }

        /// <summary>
        /// Resolves all the instances of the requested service.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>Sequence of service instance objects.</returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            IEnumerable<Lazy<object, object>> exports = this.compositionContainer.GetExports(serviceType, null, key);
            if ((exports != null) && (exports.Count() > 0))
            {
                // If there is more than one value, this will throw an InvalidOperationException, 
                // which will be wrapped by the base class as an ActivationException.
                return exports.Single().Value;
            }

            throw new ActivationException(
                this.FormatActivationExceptionMessage(new CompositionException("Export not found"), serviceType, key));
        }
    }
}