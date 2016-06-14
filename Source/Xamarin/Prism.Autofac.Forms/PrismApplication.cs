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
using Prism;
using Prism.Autofac.Forms;

namespace Prism.Autofac
{
    public abstract class PrismApplication : PrismApplicationBase<IContainer>
    {
        const string _navigationServiceName = "AutofacPageNavigationService";

        public PrismApplication(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                NamedParameter parameter = null;
                var page = view as Page;
                if (page != null)
                {
                    var navService = CreateNavigationService();
                    ((IPageAware)navService).Page = page;

                    parameter = new NamedParameter("navigationService", navService);
                }

                return Container.Resolve(type, parameter);
            });
        }

        protected override IContainer CreateContainer()
        {
            return new ContainerBuilder().Build();
        }

        protected override IModuleManager CreateModuleManager()
        {
            return Container.Resolve<IModuleManager>();
        }

        protected override INavigationService CreateNavigationService()
        {
            return Container.ResolveNamed<INavigationService>(_navigationServiceName);
        }

        protected override void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(Logger).As<ILoggerFacade>();
            builder.RegisterInstance(ModuleCatalog).As<IModuleCatalog>();

            builder.Register(ctx => new ApplicationProvider()).As<IApplicationProvider>().SingleInstance();
            builder.Register(ctx => new AutofacPageNavigationService(Container, Container.Resolve<IApplicationProvider>(), Container.Resolve<ILoggerFacade>())).Named<INavigationService>(_navigationServiceName);
            builder.Register(ctx => new ModuleManager(Container.Resolve<IModuleInitializer>(), Container.Resolve<IModuleCatalog>())).As<IModuleManager>();
            builder.Register(ctx => new AutofacModuleInitializer(Container)).As<IModuleInitializer>();
            builder.Register(ctx => new EventAggregator()).As<IEventAggregator>();
            builder.Register(ctx => new DependencyService()).As<IDependencyService>();
            builder.Register(ctx => new PageDialogService(ctx.Resolve<IApplicationProvider>())).As<IPageDialogService>();

            builder.Update(Container);
        }
    }
}
