using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Common;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Unity.Extensions;
using Prism.Unity.Navigation;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.Unity
{
    public abstract class UnityBootstrapper : Bootstrapper
    {
        public IUnityContainer Container { get; protected set; }

        public override void Run()
        {
            Logger = CreateLogger();

            Container = CreateContainer();

            ConfigureContainer();
            RegisterTypes();

            App.MainPage = CreateMainPage();
            InitializeMainPage();
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                ParameterOverrides overrides = null;

                var page = view as Page;
                if (page != null)
                {
                    var navService = Container.Resolve<UnityPageNavigationService>();
                    ((IPageAware)navService).Page = page;

                    overrides = new ParameterOverrides
                    {
                        { "navigationService", navService }
                    };
                }

                return Container.Resolve(type, overrides);
            });
        }

        protected virtual void InitializeMainPage()
        {
        }

        protected virtual IUnityContainer CreateContainer()
        {
            return new UnityContainer();
        }

        protected virtual void ConfigureContainer()
        {
            Container.AddNewExtension<DependencyServiceExtension>();

            Container.RegisterInstance<ILoggerFacade>(Logger);

            Container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDependencyService, DependencyService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPageDialogService, PageDialogService>(new ContainerControlledLifetimeManager());
        }
    }
}
