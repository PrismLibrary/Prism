using Prism.Ioc;
using Unity;

namespace Prism.Unity
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        public override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension(new UnityContainer());
        }
    }
}
