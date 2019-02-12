using Prism.Ioc;
using Unity;

namespace Prism.Unity
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension(new UnityContainer());
        }
    }
}
