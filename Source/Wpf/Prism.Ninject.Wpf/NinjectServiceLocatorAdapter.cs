using System;
using System.Collections.Generic;
using Ninject;
using CommonServiceLocator;

namespace Prism.Ninject
{
    internal class NinjectServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly IKernel Kernel;

        public NinjectServiceLocatorAdapter(IKernel kernel)
        {
            this.Kernel = kernel;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return this.Kernel.GetAll(serviceType);
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return this.Kernel.Get(serviceType, name: key);
        }
    }
}
