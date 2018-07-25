
using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using Prism.Navigation;
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
