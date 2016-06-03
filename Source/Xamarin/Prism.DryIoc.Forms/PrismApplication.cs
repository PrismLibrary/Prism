using System;
using System.Linq;
using DryIoc;
using Prism.Common;
using Prism.DryIoc.Extensions;
using Prism.DryIoc.Modularity;
using Prism.DryIoc.Navigation;
using Prism.Events;
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
    public abstract class PrismApplication : PrismApplicationBase
    {
        public IContainer Container { get; protected set; }

        private const string _navigationServiceKey = "DryIocPageNavigationService";

        public override void Initialize()
        {
            Logger = CreateLogger();

            ModuleCatalog = CreateModuleCatalog();
            ConfigureModuleCatalog();

            Container = CreateContainer();
            ConfigureContainer();
            NavigationService = CreateNavigationService();

            RegisterTypes();

            InitializeModules();

            OnInitialized();
        }

        /// <summary>
        /// Create a default instance of <see cref="IContainer" /> with <see cref="Rules" /> created in
        /// <see cref="CreateContainerRules" />
        /// </summary>
        /// <returns>An instance of <see cref="IContainer" /></returns>
        protected virtual IContainer CreateContainer()
        {
            var rules = CreateContainerRules();
            return new Container(rules);
        }

        /// <summary>
        /// Create <see cref="Rules" /> to alter behavior of <see cref="IContainer" />
        /// </summary>
        /// <remarks>
        /// Default rule is to consult <see cref="Xamarin.Forms.DependencyService" /> if the requested type cannot be inferred from
        /// <see cref="Container" />
        /// </remarks>
        /// <returns>An instance of <see cref="Rules" /></returns>
        protected virtual Rules CreateContainerRules()
        {
            return UnknownServiceResolverRule.DependencyServiceResolverRule;
        }

        protected virtual void ConfigureContainer()
        {
            Container.RegisterInstance(Logger);
            Container.RegisterInstance(ModuleCatalog);
            Container.RegisterInstance(Container);
            Container.Register<INavigationService, DryIocPageNavigationService>(
                serviceKey: _navigationServiceKey,
                setup: Setup.With(allowDisposableTransient: true));
            Container.Register<IApplicationProvider, ApplicationProvider>();
            Container.Register<IModuleManager, ModuleManager>(Reuse.Singleton);
            Container.Register<IModuleInitializer, DryIocModuleInitializer>(Reuse.Singleton);
            Container.Register<IEventAggregator, EventAggregator>(Reuse.Singleton);
            Container.Register<IDependencyService, DependencyService>(Reuse.Singleton);
            Container.Register<IPageDialogService, PageDialogService>(Reuse.Singleton);
        }

        protected override void InitializeModules()
        {
            if (ModuleCatalog.Modules.Any())
            {
                var manager = Container.Resolve<IModuleManager>();
                manager.Run();
            }
        }

        /// <summary>
        /// Create instance of <see cref="INavigationService"/>
        /// </summary>
        /// <remarks>
        /// The <see cref="_navigationServiceKey"/> is used as service key when resolving
        /// </remarks>
        /// <returns>Instance of <see cref="INavigationService"/></returns>
        protected override INavigationService CreateNavigationService()
        {
            return Container.Resolve<INavigationService>(_navigationServiceKey);
        }

        /// <summary>
        /// Create instance of <see cref="INavigationService"/> and set the <see cref="IPageAware.Page"/> property to <paramref name="page"/>
        /// </summary>
        /// <param name="page">Active page</param>
        /// <returns>Instance of <see cref="INavigationService"/> with <see cref="IPageAware.Page"/> set</returns>
        protected INavigationService CreateNavigationService(Page page)
        {
            var navigationService = CreateNavigationService();
            ((IPageAware)navigationService).Page = page;
            return navigationService;
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                var page = view as Page;
                if (page != null)
                {
                    return PageViewModelFactory(page, type);
                }
                return Container.Resolve(type);
            });
        }

        /// <summary>
        /// Resolve the view model of <paramref name="type"/> associated with <paramref name="page"/>
        /// </summary>
        /// <remarks>
        /// The method will set the <see cref="IPageAware.Page" /> property on the <see cref="INavigationService"/>  
        /// instance that will be injected into the view model.
        /// </remarks>
        /// <param name="page">The <see cref="Page"/> associated with the view model</param>
        /// <param name="type">View model type to resolve</param>
        /// <returns>View model instance of type <paramref name="type"/></returns>
        protected virtual object PageViewModelFactory(Page page, Type type)
        {
            var navigationService = CreateNavigationService(page);
            // Resolve type using the instance navigationService
            var resolver = Container.Resolve<Func<INavigationService, object>>(type);
            return resolver(navigationService);
        }
    }
}