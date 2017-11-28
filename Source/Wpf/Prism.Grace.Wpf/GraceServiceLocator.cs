using CommonServiceLocator;
using Grace.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Grace.Wpf
{
    /// <summary> 
    ///     Grace implementation of the Microsoft CommonServiceLocator. 
    /// </summary> 
    public class GraceServiceLocator : ServiceLocatorImplBase
    {
        private readonly DependencyInjectionContainer container;

        public GraceServiceLocator(DependencyInjectionContainer container)
        {
            this.container = container ?? throw new ArgumentNullException("container");
        }

        public DependencyInjectionContainer GetContainer()
        {
            return container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            return key != null ? container.Locate(serviceType, withKey: key) : container.Locate(serviceType);
        }
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            return container.LocateAll(serviceType).AsEnumerable();
        }

    }
}