using Ninject;
using Ninject.Parameters;
using Ninject.Planning.Bindings.Resolvers;
using Prism.Common;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Ninject.Extensions;
using Prism.Ninject.Modularity;
using Prism.Ninject.Navigation;
using Prism.Services;
using System.Linq;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.Ninject
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        /// <summary>
        /// The Ninject Kernel
        /// </summary>
        public IKernel Kernel { get; protected set; }

        public override void Initialize()
        {
            Logger = CreateLogger();

            ModuleCatalog = CreateModuleCatalog();
            ConfigureModuleCatalog();

            Kernel = CreateKernel();

            ConfigureKernel();

            NavigationService = CreateNavigationService();

            RegisterTypes();

            InitializeModules();

            OnInitialized();
        }

        /// <inheritDoc />
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                IParameter[] overrides = null;

                var page = view as Page;
                if (page != null)
                {
                    var navService = Kernel.Get<NinjectNavigationService>();
                    ((IPageAware)navService).Page = page;

                    overrides = new IParameter[]
                    {
                        new ConstructorArgument( "navigationService", navService )
                    };
                }

                return Kernel.Get(type, overrides);
            });
        }

        /// <summary>
        /// Override to change the creation of the Ninject kernel.
        /// If you are using <see cref="Xamarin.Forms.DependencyService"/>,
        /// you should return a <see cref="Prism.Ninject.Extensions.DependencyServiceKernel"/>.
        /// </summary>
        /// <returns>A Ninject <see cref="IKernel"/></returns>
        protected virtual IKernel CreateKernel()
        {
            return new StandardKernel();
        }

        /// <summary>
        /// Override to add your own Ninject kernel bindings. Make sure you call to base.
        /// </summary>
        protected virtual void ConfigureKernel()
        {
            Kernel.Components.Add<IMissingBindingResolver, DependencyServiceBindingResolver>();

            Kernel.Bind<ILoggerFacade>().ToConstant(Logger).InSingletonScope();
            Kernel.Bind<IModuleCatalog>().ToConstant(ModuleCatalog).InSingletonScope();

            Kernel.Bind<IModuleManager>().To<ModuleManager>().InSingletonScope();
            Kernel.Bind<IModuleInitializer>().To<NinjectModuleInitializer>().InSingletonScope();
            Kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Kernel.Bind<IDependencyService>().To<DependencyService>().InSingletonScope();
            Kernel.Bind<IPageDialogService>().To<PageDialogService>().InSingletonScope();
        }

        protected override INavigationService CreateNavigationService()
        {
            return Kernel.Get<NinjectNavigationService>();
        }

        protected override void InitializeModules()
        {
            if (ModuleCatalog.Modules.Count() > 0)
            {
                IModuleManager manager = Kernel.Get<IModuleManager>();
                manager.Run();
            }
        }
    }
}
