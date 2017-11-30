using Grace.DependencyInjection;
using Prism.Ioc;

namespace Prism.Grace
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        public PrismApplication(IPlatformInitializer initializer = null)
            : base(initializer) { }

        protected override IContainerExtension CreateContainerExtension()
        {
            return new GraceContainerExtension(new DependencyInjectionContainer());
        }
    }
}