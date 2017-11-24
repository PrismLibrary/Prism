using System;
using System.Collections.Generic;
using System.Text;
using Ninject;
using Prism.Ioc;

namespace Prism.Ninject
{
    public class NinjectContainerExtension : IContainerExtension<IKernel>
    {
        public NinjectContainerExtension(IKernel kernel)
        {
            Instance = kernel;
        }

        public IKernel Instance { get; }
    }
}
