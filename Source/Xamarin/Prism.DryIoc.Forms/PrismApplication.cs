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
        /// Create a default instance of <see cref="IContainer"/> with <see cref="Rules"/> created in <see cref="CreateContainerRules"/>
        /// </summary>
        /// <returns>An instance of <see cref="IContainer"/></returns>
        protected virtual IContainer CreateContainer()
        {
            var rules = CreateContainerRules();
            return new Container(rules);
        }

        /// <summary>
        /// Create <see cref="Rules"/> to alter behavior of <see cref="IContainer"/>
        /// </summary>
        /// <remarks>
        /// Default rule is to consult <see cref="Xamarin.Forms.DependencyService"/> if the requested type cannot be inferred from <see cref="Container"/>
        /// </remarks>
        /// <returns>An instance of <see cref="Rules"/></returns>
        protected virtual Rules CreateContainerRules()
        {
            return UnknownServiceResolverRule.DependencyServiceResolverRule;
        }

        protected virtual void ConfigureContainer()
        {
            Container.RegisterInstance(Logger);
            Container.RegisterInstance(ModuleCatalog);
            Container.RegisterInstance(NavigationService);
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

        protected override INavigationService CreateNavigationService()
        {
            return new DryIocPageNavigationService(Container);
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                var page = view as Page;
                if (page != null)
                {
                    var navigationService = Container.Resolve<DryIocPageNavigationService>();
                    ((IPageAware)navigationService).Page = page;
                }
                return Container.Resolve(type);
            });
        }
    }
}
