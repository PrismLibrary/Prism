using Prism.Ioc;
using Unity;

namespace Prism.Unity
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        public sealed override IContainerExtension CreateContainer()
        {
            return new UnityContainerExtension(new UnityContainer());
        }
    }
}
