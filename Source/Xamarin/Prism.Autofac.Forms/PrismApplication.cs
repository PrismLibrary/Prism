using System;
using System.Linq;
using System.Reflection;
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
using Autofac.Features.ResolveAnything;
using Prism.Autofac.Forms.Modularity;
using Prism.Autofac.Navigation;
using Prism.Autofac.Forms;
using Prism.AppModel;
using Prism.Autofac.Forms.Immutable;

// ReSharper disable once CheckNamespace
namespace Prism.Autofac
{
    /// <summary>
    /// Application base class using Autofac
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase<IAutofacContainer>
    {
        /// <summary>
        /// Service key used when registering the <see cref="AutofacPageNavigationService"/> with the container
        /// </summary>
        // ReSharper disable once InconsistentNaming
        const string _navigationServiceName = "AutofacPageNavigationService";

        private IAutofacContainer _immutableContainer;
        private IApplicationProvider _immutableApplicationProvider;
        private INavigationService _initialNavigationService;
        private bool _doModuleManagerRun;

        /// <summary>
        /// Create a new instance of <see cref="PrismApplication"/>
        /// </summary>
        /// <param name="initializer">Class to initialize platform instances</param>
        /// <remarks>
        /// The method <see cref="IPlatformInitializer.RegisterTypes(IAutofacContainer)"/> will be called after <see cref="PrismApplication.RegisterTypes()"/> 
        /// to allow for registering platform specific instances.
        /// </remarks>
        protected PrismApplication(IPlatformInitializer initializer = null)
            : base(initializer)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            FinishContainerConfiguration();
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                NamedParameter parameter = null;
                if (view is Page page)
                {
                    parameter = new NamedParameter("navigationService", CreateNavigationService(page));
                }

                return Container.Resolve(type, parameter);
            });
        }

        /// <summary>
        /// Create a default instance of <see cref="IAutofacContainer" />
        /// </summary>
        /// <returns>An instance of <see cref="IAutofacContainer" /></returns>
        protected override IAutofacContainer CreateContainer()
        {
            return (_immutableContainer = _immutableContainer ?? new AutofacContainer());
        }

        protected override IModuleManager CreateModuleManager()
        {
            return Container.Resolve<IModuleManager>();
        }

        /// <summary>
        /// Create instance of <see cref="INavigationService"/>
        /// </summary>
        /// <remarks>
        /// The <see cref="_navigationServiceName"/> is used as service key when resolving
        /// </remarks>
        /// <returns>Instance of <see cref="INavigationService"/></returns>
        protected override INavigationService CreateNavigationService()
        {
            return (Container.IsContainerBuilt)
                ? Container.ResolveNamed<INavigationService>(_navigationServiceName)
                : _initialNavigationService;
        }

        protected override void InitializeModules()
        {
            //In immutable mode, module initialization is moved to the FinishContainerConfiguration() method
            _doModuleManagerRun = ModuleCatalog.Modules.Any();
        }

        protected override void ConfigureContainer()
        {
            _immutableApplicationProvider = _immutableApplicationProvider ?? new ApplicationProvider();
            _initialNavigationService = _initialNavigationService ??
                                        new AutofacPageNavigationService(null, _immutableApplicationProvider, Logger);

            Container.RegisterInstance(Logger).As<ILoggerFacade>().SingleInstance();
            Container.RegisterInstance(ModuleCatalog).As<IModuleCatalog>().SingleInstance();
            Container.RegisterInstance(_immutableApplicationProvider).As<IApplicationProvider>().SingleInstance();
            Container.Register(ctx => new ApplicationStore()).As<IApplicationStore>().SingleInstance();
            Container.Register(ctx => new AutofacPageNavigationService(Container, Container.Resolve<IApplicationProvider>(), Container.Resolve<ILoggerFacade>()))
                .Named<INavigationService>(_navigationServiceName);
            Container.Register(ctx => new AutofacModuleInitializer(Container)).As<IModuleInitializer>().SingleInstance();
            Container.Register(ctx => new ModuleManager(Container.Resolve<IModuleInitializer>(), Container.Resolve<IModuleCatalog>()))
                .As<IModuleManager>().SingleInstance();
            Container.Register(ctx => new EventAggregator()).As<IEventAggregator>().SingleInstance();
            Container.Register(ctx => new DependencyService()).As<IDependencyService>().SingleInstance();
            Container.Register(ctx => new PageDialogService(ctx.Resolve<IApplicationProvider>())).As<IPageDialogService>().SingleInstance();
            Container.Register(ctx => new DeviceService()).As<IDeviceService>().SingleInstance();
            Container.RegisterInstance(Container).As<IContainer>().SingleInstance();
            Container.RegisterInstance(Container).As<IAutofacContainer>().SingleInstance();
            Container.Register(ctx => CreateNavigationService()).As<INavigationService>();
        }

        private void PreRegisterModuleTypes()
        {
            foreach (Type moduleType in ModuleCatalog
                .Modules
                .Select(s => s.ModuleType)
                .Where(w => w != null && w.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IPreRegisterTypes)))
                .Distinct())
            {
                object instance = null;
                foreach (ConstructorInfo ctor in moduleType.GetTypeInfo().DeclaredConstructors)
                {
                    ParameterInfo[] ctorParams = ctor.GetParameters();
                    if (ctorParams == null || ctorParams.Length == 0)
                    {
                        instance = ctor.Invoke(new object[] { });
                    }
                    else if (ctorParams.Length == 1 && (ctorParams[0].ParameterType == typeof(IContainer) ||
                                                        ctorParams[0].ParameterType == typeof(IAutofacContainer)))
                    {
                        instance = ctor.Invoke(new object[] { Container });
                    }
                }
                if (instance == null)
                {
                    throw new InvalidOperationException(
                        $"Unable to execute RegisterTypes() on the '{moduleType.Name}' module because a compatible constructor could not be found.");
                }
                (instance as IPreRegisterTypes)?.RegisterTypes(Container);
            }
        }

        /// <summary>
        /// Finish the container's configuration after all other types are registered.
        /// </summary>
        private void FinishContainerConfiguration()
        {
            if (_doModuleManagerRun)
            {
                //Pre-registering any module types here - using reflection to create an instance of the module and run RegisterTypes() on it
                //  because the container has not been built yet; so I can't use the container to give me an instance of the module.
                PreRegisterModuleTypes();
            }

            Container.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            (_initialNavigationService as AutofacPageNavigationService)?.SetContainer(Container);

            if (_doModuleManagerRun)
            {
                //Finished registering things in the container, so the container can be built and modules initialized
                Container.Resolve<IModuleManager>().Run();
            }
        }
    }
}
