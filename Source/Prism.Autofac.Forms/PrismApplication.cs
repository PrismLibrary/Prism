using System.Linq;
using Prism.Navigation;
using Prism.Mvvm;
using Prism.Common;
using Xamarin.Forms;
using Prism.Logging;
using Prism.Events;
using Prism.Services;
using DependencyService = Prism.Services.DependencyService;
using Prism.Modularity;
using Autofac;
using Prism.Autofac.Forms.Modularity;
using System;
using System.Globalization;
using Prism.Autofac.Navigation;

namespace Prism.Autofac
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        public IContainer Container { get; protected set; }

        public override void Initialize()
        {
            Logger = CreateLogger();

            ModuleCatalog = CreateModuleCatalog();
            ConfigureModuleCatalog();

            Container = CreateAndConfigureContainer();

            NavigationService = CreateNavigationService();

            RegisterTypes();

            InitializeModules();
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                NamedParameter parameter = null;
                var page = view as Page;
                if (page != null)
                {
                    var navService = Container.Resolve<AutofacPageNavigationService>();
                    ((IPageAware)navService).Page = page;

                    parameter = new NamedParameter("navigationService", navService);
                }

                return Container.Resolve(type, parameter);
            });
        }

        protected override INavigationService CreateNavigationService()
        {
            return Container.Resolve<AutofacPageNavigationService>();
        }

        protected virtual IContainer CreateAndConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(Logger).As<ILoggerFacade>();
            builder.RegisterInstance(ModuleCatalog).As<IModuleCatalog>();

            builder.Register(ctx => new AutofacModuleInitializer(ctx)).As<IModuleInitializer>();
            builder.Register(ctx => new ModuleManager(ctx.Resolve<IModuleInitializer>(), ctx.Resolve<IModuleCatalog>())).As<IModuleManager>();
            builder.Register(ctx => new EventAggregator()).As<IEventAggregator>();
            builder.Register(ctx => new DependencyService()).As<IDependencyService>();
            builder.Register(ctx => new PageDialogService()).As<IPageDialogService>();

            return builder.Build();
        }

        protected override void InitializeModules()
        {
            if (ModuleCatalog.Modules.Count() > 0)
            {
                IModuleManager manager = Container.Resolve<IModuleManager>();
                manager.Run();
            }
        }
    }
}
