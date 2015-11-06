﻿using Microsoft.Practices.ServiceLocation;
using Ninject;
using Ninject.Parameters;
using Ninject.Planning.Bindings.Resolvers;
using Prism.Common;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Ninject.Extensions;
using Prism.Services;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.Ninject
{
    /// <summary>
    /// A Ninject specific bootstrap class
    /// </summary>
    public abstract class NinjectBootstrapper : Bootstrapper
    {
        /// <summary>
        /// The Ninject Kernel
        /// </summary>
        public IKernel Kernel { get; protected set; }


        /// <inheritDoc />
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

        /// <inheritDoc />
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

        /// <inheritDoc />
        protected virtual void InitializeMainPage()
        {
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

            Kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Kernel.Bind<IServiceLocator>().To<NinjectServiceLocatorAdapter>().InSingletonScope();
            Kernel.Bind<IDependencyService>().To<DependencyService>().InSingletonScope();
            Kernel.Bind<IPageDialogService>().To<PageDialogService>().InSingletonScope();
        }

        /// <inheritDoc />
        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => this.Kernel.Get<IServiceLocator>());
        }
    }
}
