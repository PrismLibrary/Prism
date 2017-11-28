using Ninject;
using Prism.Ioc;

namespace Prism.Ninject
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        public PrismApplication(IPlatformInitializer initializer = null) 
            : base(initializer) { }

        protected override IContainerExtension CreateContainerExtension()
        {
            return new NinjectContainerExtension(new StandardKernel());
        }
    }
}
