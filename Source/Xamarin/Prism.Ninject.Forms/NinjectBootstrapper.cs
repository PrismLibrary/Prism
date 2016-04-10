using Ninject;
using Ninject.Parameters;
using Ninject.Planning.Bindings.Resolvers;
using Prism.Common;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Ninject.Extensions;
using Prism.Ninject.Navigation;
using Prism.Services;
using System;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.Ninject
{
    /// <summary>
    /// A Ninject specific bootstrap class
    /// </summary>
    [Obsolete("Please have your App.cs derive from PrismApplication instead.")]
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

            NavigationService = CreateNavigationService();

            RegisterTypes();

            //****** Obsolete ******//
            var page = CreateMainPage();
            if (page != null)
                App.MainPage = page;

            InitializeMainPage();
            //**********************//

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

        /// <inheritDoc />
        [Obsolete]
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
            Kernel.Bind<IDependencyService>().To<DependencyService>().InSingletonScope();
            Kernel.Bind<IPageDialogService>().To<PageDialogService>().InSingletonScope();
        }

        protected override INavigationService CreateNavigationService()
        {
            return Kernel.Get<NinjectNavigationService>();
        }
    }
}
