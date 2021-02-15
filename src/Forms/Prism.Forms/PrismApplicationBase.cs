using System;
using System.Collections.Generic;
using System.Linq;
using Prism.AppModel;
using Prism.Behaviors;
using Prism.Common;
using Prism.Events;
using Prism.Extensions;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Prism
{
    /// <summary>
    /// The Base implementation for a PrismApplication
    /// </summary>
    public abstract class PrismApplicationBase : Application
    {
        /// <summary>
        /// Gets the Current PrismApplication
        /// </summary>
        public new static PrismApplicationBase Current => (PrismApplicationBase)Application.Current;

        /// <summary>
        /// The registration name to create a new transient instance of the <see cref="INavigationService"/>
        /// </summary>
        public const string NavigationServiceName = "PageNavigationService";

        private IContainerExtension _containerExtension;
        private IModuleCatalog _moduleCatalog;
        private Page _previousPage = null;

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        public IContainerProvider Container => _containerExtension;

        /// <summary>
        /// Gets the <see cref="INavigationService"/> for the application.
        /// </summary>
        protected INavigationService NavigationService { get; set; }

        /// <summary>
        /// Get the Platform Initializer
        /// </summary>
        protected IPlatformInitializer PlatformInitializer { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplicationBase" /> using the default constructor
        /// </summary>
        protected PrismApplicationBase() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplicationBase" /> with a <see cref="IPlatformInitializer" />.
        /// Used when there are specific types that need to be registered on the platform.
        /// </summary>
        /// <param name="platformInitializer">The <see cref="IPlatformInitializer"/>.</param>
        protected PrismApplicationBase(IPlatformInitializer platformInitializer)
        {
            base.ModalPopping += PrismApplicationBase_ModalPopping;
            base.ModalPopped += PrismApplicationBase_ModalPopped;

            PlatformInitializer = platformInitializer;
            InitializeInternal();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplicationBase" /> with a <see cref="IPlatformInitializer" />.
        /// Used when there are specific types that need to be registered on the platform.
        /// Also determines whether to set the <see cref="DependencyResolver" /> for resolving Renderers and Platform Effects.
        /// </summary>
        /// <param name="platformInitializer">The <see cref="IPlatformInitializer"/>.</param>
        /// <param name="setFormsDependencyResolver">Should <see cref="PrismApplicationBase" /> set the <see cref="DependencyResolver" />.</param>
        [Obsolete]
        protected PrismApplicationBase(IPlatformInitializer platformInitializer, bool setFormsDependencyResolver)
            : this(platformInitializer)
        {
        }

        /// <summary>
        /// Run the initialization process.
        /// </summary>
        private void InitializeInternal()
        {
            ConfigureViewModelLocator();
            Initialize();
            OnInitialized();
        }

        /// <summary>
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                List<(Type Type, object Instance)> overrides = new List<(Type, object)>();
                if (Container.IsRegistered<IResolverOverridesHelper>())
                {
                    var resolver = Container.Resolve<IResolverOverridesHelper>();
                    var resolverOverrides = resolver.GetOverrides();
                    if (resolverOverrides.Any())
                        overrides.AddRange(resolverOverrides);
                }

                if (!overrides.Any(x => x.Type == typeof(INavigationService)))
                {
                    var navService = CreateNavigationService(view);
                    overrides.Add((typeof(INavigationService), navService));
                }

                return Container.Resolve(type, overrides.ToArray());
            });
        }

        private INavigationService CreateNavigationService(object view)
        {
            if (view is Page page)
            {
                return Navigation.Xaml.Navigation.GetNavigationService(page);
            }
            else if (view is VisualElement visualElement && visualElement.TryGetParentPage(out var parent))
            {
                return Navigation.Xaml.Navigation.GetNavigationService(parent);
            }

            return Container.Resolve<INavigationService>();
        }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        protected virtual void Initialize()
        {
            ContainerLocator.SetContainerExtension(CreateContainerExtension);
            _containerExtension = ContainerLocator.Current;
            RegisterRequiredTypes(_containerExtension);
            PlatformInitializer?.RegisterTypes(_containerExtension);
            RegisterTypes(_containerExtension);
            _containerExtension.FinalizeExtension();

            _moduleCatalog = Container.Resolve<IModuleCatalog>();
            ConfigureModuleCatalog(_moduleCatalog);

            _containerExtension.CreateScope();
            NavigationService = _containerExtension.Resolve<INavigationService>();

            InitializeModules();
        }

        /// <summary>
        /// Creates the container used by Prism.
        /// </summary>
        /// <returns>The container</returns>
        protected abstract IContainerExtension CreateContainerExtension();

        /// <summary>
        /// Registers all types that are required by Prism to function with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected virtual void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IApplicationProvider, ApplicationProvider>();
            containerRegistry.RegisterSingleton<IApplicationStore, ApplicationStore>();
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.RegisterSingleton<IKeyboardMapper, KeyboardMapper>();
            containerRegistry.RegisterSingleton<IPageDialogService, PageDialogService>();
            containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            containerRegistry.RegisterSingleton<IDeviceService, DeviceService>();
            containerRegistry.RegisterSingleton<IPageBehaviorFactory, PageBehaviorFactory>();
            containerRegistry.RegisterSingleton<IModuleCatalog, ModuleCatalog>();
            containerRegistry.RegisterSingleton<IModuleManager, ModuleManager>();
            containerRegistry.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
            containerRegistry.RegisterScoped<INavigationService, PageNavigationService>();
            containerRegistry.Register<INavigationService, PageNavigationService>(NavigationServiceName);
        }

        /// <summary>
        /// Used to register types with the container that will be used by your application.
        /// </summary>
        protected abstract void RegisterTypes(IContainerRegistry containerRegistry);

        /// <summary>
        /// Configures the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        /// <param name="moduleCatalog">The ModuleCatalog to configure</param>
        protected virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) { }

        /// <summary>
        /// Initializes the modules.
        /// </summary>
        protected virtual void InitializeModules()
        {
            if (_moduleCatalog.Modules.Any())
            {
                IModuleManager manager = Container.Resolve<IModuleManager>();
                manager.Run();
            }
        }

        /// <summary>
        /// Called when the PrismApplication has completed it's initialization process.
        /// </summary>
        protected abstract void OnInitialized();

        /// <summary>
        /// Application developers override this method to perform actions when the application
        /// resumes from a sleeping state
        /// </summary>
        /// <remarks>
        /// Be sure to call base.OnResume() or you will lose support for IApplicationLifecycleAware
        /// </remarks>
        protected override void OnResume()
        {
            if (MainPage != null)
            {
                var page = PageUtilities.GetCurrentPage(MainPage);
                PageUtilities.InvokeViewAndViewModelAction<IApplicationLifecycleAware>(page, x => x.OnResume());
            }
        }

        /// <summary>
        /// Application developers override this method to perform actions when the application
        /// enters the sleeping state
        /// </summary>
        /// <remarks>
        /// Be sure to call base.OnSleep() or you will lose support for IApplicationLifecycleAware
        /// </remarks>
        protected override void OnSleep()
        {
            if (MainPage != null)
            {
                var page = PageUtilities.GetCurrentPage(MainPage);
                PageUtilities.InvokeViewAndViewModelAction<IApplicationLifecycleAware>(page, x => x.OnSleep());
            }
        }

        private void PrismApplicationBase_ModalPopping(object sender, ModalPoppingEventArgs e)
        {
            if (PageNavigationService.NavigationSource == PageNavigationSource.Device)
            {
                _previousPage = PageUtilities.GetOnNavigatedToTarget(e.Modal, MainPage, true);
            }
        }

        private void PrismApplicationBase_ModalPopped(object sender, ModalPoppedEventArgs e)
        {
            if (PageNavigationService.NavigationSource == PageNavigationSource.Device)
            {
                PageUtilities.HandleSystemGoBack(e.Modal, _previousPage);
                _previousPage = null;
            }
        }
    }
}
