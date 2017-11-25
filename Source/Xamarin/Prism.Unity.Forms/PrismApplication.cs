using Prism.Ioc;
using Unity;

namespace Prism.Unity
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        public PrismApplication(IPlatformInitializer initializer = null) 
            : base (initializer) { }

        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension(new UnityContainer());
        }
    }
}
