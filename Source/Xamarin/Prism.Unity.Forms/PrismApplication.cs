using Prism.Ioc;
using Prism.Mvvm;
using Unity;
using Unity.Resolution;
using Xamarin.Forms;

namespace Prism.Unity
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        public PrismApplication(IPlatformInitializer initializer = null) 
            : base (initializer) { }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                ParameterOverrides overrides = null;

                if (view is Page page)
                {
                    overrides = new ParameterOverrides
                    {
                        { "navigationService", CreateNavigationService(page) }
                    };
                }

                return Container.GetContainer().Resolve(type, overrides);
            });
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension(new UnityContainer());
        }
    }
}
