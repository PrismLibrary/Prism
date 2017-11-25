using Autofac;
using Prism.Ioc;
using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.Autofac
{
    /// <summary>
    /// Application base class using Autofac
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase
    {
        protected PrismApplication(IPlatformInitializer platformInitializer = null)
            : base(platformInitializer) { }

        /// <summary>
        /// Configures the ViewModel Locator to resolve the ViewModel type and ensure the correct
        /// instance of <see cref="INavigationService"/> is properly injected into the ViewModel.
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                NamedParameter parameter = null;
                if (view is Page page)
                {
                    parameter = new NamedParameter("navigationService", CreateNavigationService(page));
                }

                return Container.GetContainer().Resolve(type, parameter);
            });
        }

        /// <summary>
        /// Creates the <see cref="IAutofacContainerExtension"/>
        /// </summary>
        /// <returns></returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new AutofacContainerExtension(new ContainerBuilder());
        }
    }
}
