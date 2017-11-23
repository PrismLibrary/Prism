using Prism.Ioc;
using Prism.Mvvm;
using Unity;
using Unity.Resolution;
using Xamarin.Forms;

namespace Prism.Unity
{
    public abstract class PrismApplication : PrismApplicationBase<IUnityContainer>
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

                return Container.Instance.Resolve(type, overrides);
            });
        }

        protected override IContainerExtension<IUnityContainer> CreateContainerExtension()
        {
            return new UnityContainerExtension(new UnityContainer());
        }
    }
}
