using Prism.Ioc;
using Prism.Navigation;
using Unity;
using Unity.Resolution;
using Windows.UI.Xaml.Controls;

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
