using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Navigation;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity.Navigation;
using Prism.Common;
using Xamarin.Forms;
using Prism.Unity.Extensions;
using Prism.Logging;
using Prism.Events;
using Prism.Services;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.Unity
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        public IUnityContainer Container { get; protected set; }

        public override void Initialize()
        {
            Logger = CreateLogger();

            Container = CreateContainer();

            ConfigureContainer();

            NavigationService = CreateNavigationService();

            RegisterTypes();

            OnInitialized();
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

        protected virtual IUnityContainer CreateContainer()
        {
            return new UnityContainer();
        }

        protected override INavigationService CreateNavigationService()
        {
            return Container.Resolve<UnityPageNavigationService>();
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
