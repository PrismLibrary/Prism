namespace Prism.Composition.Windows
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    
    /// <summary>A service locator adapter.</summary>
    public class ServiceLocatorAdapter : ServiceLocatorImplBase
    {
        /// <summary>initializes a new instance of the Prism.Composition.Windows.ServiceLocatorAdapter class.</summary>
        /// <param name="unityContainer">The unity container. </param>
        public ServiceLocatorAdapter(IUnityContainer unityContainer)
        {
            this.UnityContainer = unityContainer;
        }
        
        /// <summary>Gets or sets the unity container.</summary>
        /// <value>The unity container.</value>
        private IUnityContainer UnityContainer { get; set; }
        
        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving
        /// the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested. </param>
        /// <param name="key">        Name of registered service you want. May be null. </param>
        /// <returns>The requested service instance.</returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return this.UnityContainer.Resolve(serviceType, key);
        }
        
        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of
        /// resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested. </param>
        /// <returns>Sequence of service instance objects.</returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return this.UnityContainer.ResolveAll(serviceType);
        }
    }
}
