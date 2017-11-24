using System;
using System.Linq;
using DryIoc;
using Prism.AppModel;
using Prism.Behaviors;
using Prism.Common;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.DryIoc
{
    /// <summary>
    /// Application base class using DryIoc
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase<IContainer>
    {
        /// <summary>
        /// Create a new instance of <see cref="PrismApplication"/>
        /// </summary>
        /// <param name="platformInitializer">Class to initialize platform instances</param>
        /// <remarks>
        /// The method <see cref="IPlatformInitializer.RegisterTypes(IContainerRegistry)"/> will be called after <see cref="PrismApplication.RegisterTypes()"/> 
        /// to allow for registering platform specific instances.
        /// </remarks>
        protected PrismApplication(IPlatformInitializer platformInitializer = null)
            : base(platformInitializer)
        {

        }

        /// <summary>
        /// Creates the <see cref="IContainerExtension"/> for DryIoc
        /// </summary>
        /// <returns></returns>
        protected override IContainerExtension<IContainer> CreateContainerExtension()
        {
            return new DryIocContainerExtension(new Container(CreateContainerRules()));
        }

        /// <summary>
        /// Create <see cref="Rules" /> to alter behavior of <see cref="IContainer" />
        /// </summary>
        /// <remarks>
        /// Default rule is to consult <see cref="Xamarin.Forms.DependencyService" /> if the requested type cannot be inferred from
        /// <see cref="Container" />
        /// </remarks>
        /// <returns>An instance of <see cref="Rules" /></returns>
        protected virtual Rules CreateContainerRules() => 
            Rules.Default.WithAutoConcreteTypeResolution();

        /// <summary>
        /// Configures the Container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void ConfigureContainer(IContainerRegistry containerRegistry)
        {
            base.ConfigureContainer(containerRegistry);
            Container.Instance.Register<INavigationService, PageNavigationService>();
            Container.Instance.Register<INavigationService>(
                made: Made.Of(() => SetPage(Arg.Of<INavigationService>(), Arg.Of<Page>())),
                setup: Setup.Decorator);
        }


        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                switch(view)
                {
                    case Page page:
                        var getVM = Container.Instance.Resolve<Func<Page, object>>(type);
                        return getVM(page);
                    default:
                        return Container.Resolve(type);
                }
            });
        }

        internal static INavigationService SetPage(INavigationService navigationService, Page page)
        {
            if(navigationService is IPageAware pageAware)
            {
                pageAware.Page = page;
            }

            return navigationService;
        }
    }
}