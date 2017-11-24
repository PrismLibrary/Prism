using Autofac;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Autofac
{
    /// <summary>
    /// Application base class using Autofac
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase<IContainer>
    {
        /// <summary>
        /// Create a new instance of <see cref="PrismApplication"/>
        /// </summary>
        /// <param name="platformInitializer">Class to initialize platform instances</param>
        /// <remarks>
        /// The method <see cref="M:IPlatformInitializer.RegisterTypes(ContainerBuilder)"/> will be called after <see cref="M:PrismApplication.RegisterTypes()"/> 
        /// to allow for registering platform specific instances.
        /// </remarks>
        protected PrismApplication(IPlatformInitializer platformInitializer = null)
            : base(platformInitializer)
        {
        }


        /// <summary>
        /// Configures the ViewModel Locator to resolve the ViewModel type and ensure the correct
        /// instance of <see cref="INavigationService"/> is properly injected into the ViewModel.
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                NamedParameter parameter = null;
                var page = view as Page;
                if (page != null)
                {
                    parameter = new NamedParameter("navigationService", CreateNavigationService(page));
                }

                return Container.Instance.Resolve(type, parameter);
            });
        }

        /// <summary>
        /// Creates the <see cref="IAutofacContainerExtension"/>
        /// </summary>
        /// <returns></returns>
        protected override IContainerExtension<IContainer> CreateContainerExtension()
        {
            return new AutofacContainerExtension(new ContainerBuilder());
        }
    }
}
