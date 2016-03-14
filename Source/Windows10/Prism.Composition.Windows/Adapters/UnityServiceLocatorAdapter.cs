namespace Prism.Composition.Windows.Adapters
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    /// <summary> 
    /// Defines a <see cref="IUnityContainer"/> adapter for the <see cref="IServiceLocator"/> interface to be used by the Prism Library. 
    /// </summary> 
    public class UnityServiceLocatorAdapter : ServiceLocatorImplBase
    {
        /// <summary> 
        /// Initializes a new instance of the <see cref="UnityServiceLocatorAdapter"/> class. 
        /// </summary> 
        /// <param name="unityContainer">The <see cref="IUnityContainer"/> that will be used 
        /// by the <see cref="DoGetInstance"/> and <see cref="DoGetAllInstances"/> methods.</param> 
        public UnityServiceLocatorAdapter(IUnityContainer unityContainer)
        {
            this.UnityContainer = unityContainer;
        }

        /// <summary>
        /// Gets or sets the <see cref="IUnityContainer"/>
        /// </summary>
        private IUnityContainer UnityContainer { get; set; }

        /// <summary> 
        /// Resolves the instance of the requested service. 
        /// </summary> 
        /// <param name="serviceType">Type of instance requested.</param> 
        /// <param name="key">Name of registered service you want. May be null.</param> 
        /// <returns>The requested service instance.</returns> 
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return this.UnityContainer.Resolve(serviceType, key);
        }

        /// <summary> 
        /// Resolves all the instances of the requested service. 
        /// </summary> 
        /// <param name="serviceType">Type of service requested.</param> 
        /// <returns>Sequence of service instance objects.</returns> 
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return this.UnityContainer.ResolveAll(serviceType);
        }
    }
}
