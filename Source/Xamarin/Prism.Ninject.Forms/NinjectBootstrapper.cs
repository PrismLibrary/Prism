using Microsoft.Practices.ServiceLocation;
using Ninject;
using Ninject.Parameters;
using Prism.Common;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.Ninject
{
    public abstract class NinjectBootstrapper : Bootstrapper
    {
        public IKernel Kernel { get; protected set; }

        public override void Run()
        {
            Logger = CreateLogger();

            Kernel = CreateKernel();

            ConfigureKernel();
            ConfigureServiceLocator();
            RegisterTypes();

            App.MainPage = CreateMainPage();
            InitializeMainPage();
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                IParameter[] overrides = null;

                var page = view as Page;
                if (page != null)
                {
                    var navService = new PageNavigationService();
                    ((IPageAware)navService).Page = page;

                    overrides = new IParameter[]
                    {
                        new ConstructorArgument( "navigationService", navService )
                    };
                }

                return Kernel.Get(type, overrides);
            });
        }

        protected virtual void InitializeMainPage()
        {
        }

        protected virtual IKernel CreateKernel()
        {
            return new StandardKernel();
        }

        protected virtual void ConfigureKernel()
        {
            //Kernel.AddNewExtension<DependencyServiceExtension>();

            Kernel.Bind<ILoggerFacade>().ToConstant(Logger).InSingletonScope();

            Kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Kernel.Bind<IServiceLocator>().To<NinjectServiceLocatorAdapter>().InSingletonScope();
            Kernel.Bind<IDependencyService>().To<DependencyService>().InSingletonScope();
            Kernel.Bind<IPageDialogService>().To<PageDialogService>().InSingletonScope();
        }

        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => this.Kernel.Get<IServiceLocator>());
        }
    }
}
