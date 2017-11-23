using Prism.Ioc;
using Prism.Mvvm;
using Unity;
using Unity.Resolution;
using Xamarin.Forms;

namespace Prism.Unity
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        public PrismApplication(IPlatformInitializer initializer = null) : base (initializer) { }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                ParameterOverrides overrides = null;

                var page = view as Page;
                if (page != null)
                {
                    overrides = new ParameterOverrides
                    {
                        { "navigationService", CreateNavigationService(page) }
                    };
                }

                return ((IUnityContainer)Container.Instance).Resolve(type, overrides);
            });
        }

        protected override IContainer CreateContainer()
        {
            return new UnityContainerExtension(new UnityContainer());
        }
    }
}
