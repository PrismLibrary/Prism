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
            ConfigureNavigationService(_navigationServiceKey);
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
            Container.Register<IApplicationProvider, ApplicationProvider>();
            Container.Register<IModuleManager, ModuleManager>(Reuse.Singleton);
            Container.Register<IModuleInitializer, DryIocModuleInitializer>(Reuse.Singleton);
            Container.Register<IEventAggregator, EventAggregator>(Reuse.Singleton);
            Container.Register<IDependencyService, DependencyService>(Reuse.Singleton);
            Container.Register<IPageDialogService, PageDialogService>(Reuse.Singleton);
        }

        /// <summary>
        /// Register <see cref="INavigationService"/> using key <paramref name="serviceKey"/>
        /// </summary>
        /// <param name="serviceKey">Service key used to resolve <see cref="INavigationService"/></param>
        protected virtual void ConfigureNavigationService(string serviceKey)
        {
            Container.Register<INavigationService, DryIocPageNavigationService>(
                serviceKey: serviceKey,
                setup: Setup.With(allowDisposableTransient: true));
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
        /// Resolve <see cref="INavigationService"/>
        /// </summary>
        /// <remarks>
        /// The <see cref="_navigationServiceKey"/> is used as service key when resolving
        /// </remarks>
        /// <returns>Instance of <see cref="INavigationService"/></returns>
        protected override INavigationService CreateNavigationService()
        {
            return Container.Resolve<INavigationService>(_navigationServiceKey);
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                var page = view as Page;
                if (page != null)
                {
                    var navigationService = CreateNavigationService();
                    ((IPageAware)navigationService).Page = page;
                    ResolveTypeForPage(page, type, navigationService);
                    // Resolve type using the instance navigationService
                    var resolver = Container.Resolve<Func<INavigationService, object>>(type);
                    return resolver(navigationService);
                }
                return Container.Resolve(type);
            });
        }

        /// <summary>
        /// Called from <see cref="ViewModelLocationProvider.SetDefaultViewModelFactory(System.Func{System.Type,object})" /> when
        /// requested to resolve <paramref name="type" /> while navigatin to <see cref="view" />
        /// </summary>
        /// <remarks>
        /// This is used for testing to ensure that the resolved instance of <paramref name="navigationService" /> contains the
        /// correct instance of <paramref name="view" />
        /// </remarks>
        /// <param name="view"><see cref="Page" /> navigated to</param>
        /// <param name="type"><see cref="Type" /> to resolve</param>
        /// <param name="navigationService">Overriding instance of <see cref="INavigationService" /></param>
        protected virtual void ResolveTypeForPage(Page view, Type type, INavigationService navigationService)
        {
        }
    }
}