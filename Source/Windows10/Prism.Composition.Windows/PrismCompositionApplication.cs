namespace Prism.Composition.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;
    using Adapters;
    using Events;
    using Logging;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Prism.Windows;
    using Prism.Windows.AppModel;
    using Prism.Windows.Navigation;
    using global::Windows.ApplicationModel;
    using global::Windows.ApplicationModel.Activation;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    /// <summary>
    /// Provides the base class for the Universal Windows Platform application object which
    /// includes the automatic creation and wiring of the Unity container, MEF container and 
    /// the bootstrapping process for Prism services in the container.
    /// </summary>
    public abstract class PrismCompositionApplication : PrismApplication, IDisposable
    {
        /// <summary>
        /// Allow strongly typed access to the Application as a global
        /// </summary>
        public static new PrismCompositionApplication Current => (PrismCompositionApplication)Application.Current;

        /// <summary>
        /// Gets and (sets) the IoC Unity Container 
        /// </summary>
        public IUnityContainer Container { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Container != null)
            {
                this.Container.Dispose();

                this.Container = null;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates the <see cref="IUnityContainer"/> that will be used as the default container.
        /// Also configures the composition integration and service locator.
        /// </summary>
        /// <returns>A new instance of <see cref="IUnityContainer"/>.</returns>
        protected virtual async Task CreateAndConfigureContainerAsync()
        {
            this.Logger.Log("Creating Container", Category.Debug, Priority.Low);

            this.Container = this.CreateContainer();

            if (this.Container == null)
            {
                throw new InvalidOperationException("Unity container is null");
            }

            this.Logger.Log("Configuring Container", Category.Debug, Priority.Low);

            await this.ConfigureContainerAsync();

            this.Logger.Log("Configuring ServiceLocator", Category.Debug, Priority.Low);

            this.ConfigureServiceLocator();
        }

        /// <summary>
        /// Initializes the Frame and its content.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        /// <returns>A task of a Frame that holds the app content.</returns>
        protected override async Task<Frame> InitializeFrameAsync(IActivatedEventArgs args)
        {
            await this.CreateAndConfigureContainerAsync();

            return await base.InitializeFrameAsync(args);
        }

        /// <summary>
        /// Implements the Resolves method to be handled by the Unity Container.
        /// Use the container to resolve types so their dependencies get injected
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A concrete instance of the specified type.</returns>
        protected override object Resolve(Type type)
        {
            return this.Container.Resolve(type);
        }

        /// <summary>
        /// Creates the <see cref="IUnityContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="IUnityContainer"/>.</returns>
        protected virtual IUnityContainer CreateContainer()
        {
            return new UnityContainer();
        }

        /// <summary>
        /// Configures the container
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        protected virtual async Task ConfigureContainerAsync()
        {
            this.Logger.Log("Adding UnityExtensions to container", Category.Debug, Priority.Low);

            this.Container.RegisterAssemblies(await this.GetAssembliesAsync());

            this.Logger.Log("Setting up ServiceLocator", Category.Debug, Priority.Low);

            this.RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);

            this.Logger.Log("Registering Prism services with container", Category.Debug, Priority.Low);

            this.Container.RegisterInstance(this.Logger);

            this.RegisterTypeIfMissing(typeof(ISessionStateService), typeof(SessionStateService), true);

            this.RegisterTypeIfMissing(typeof(IDeviceGestureService), typeof(DeviceGestureService), true);

            this.RegisterTypeIfMissing(typeof(IEventAggregator), typeof(EventAggregator), true);
        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator" />.
        /// </summary>
        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => this.Container.Resolve<IServiceLocator>());
        }

        /// <summary>
        /// Creates the navigation service through the base class and registers it with the container
        /// </summary>
        /// <param name="rootFrame">The frame where navigation happens</param>
        /// <param name="sessionStateService">The session state service</param>
        /// <returns>The NavigationService</returns>
        protected override INavigationService CreateNavigationService(IFrameFacade rootFrame, ISessionStateService sessionStateService)
        {
            var navigationService = base.CreateNavigationService(rootFrame, sessionStateService);

            this.Container.RegisterInstance<INavigationService>(navigationService);

            return navigationService;
        }

        /// <summary>
        /// Creates the SessionStateService as a singleton through the container
        /// </summary>
        /// <returns>The SessionStateService</returns>
        protected override ISessionStateService OnCreateSessionStateService()
        {
            return this.Container.Resolve<ISessionStateService>();
        }

        /// <summary>
        /// Creates the DeviceGestureService as a singleton through the container
        /// </summary>
        /// <returns>DeviceGestureService instance</returns>
        protected override IDeviceGestureService OnCreateDeviceGestureService()
        {
            var deviceGestureService = this.Container.Resolve<IDeviceGestureService>();

            deviceGestureService.UseTitleBarBackButton = true;

            return deviceGestureService;
        }

        /// <summary>
        /// Creates the IEventAggregator as a singleton through the container
        /// </summary>
        /// <returns>IEventAggregator instance</returns>
        protected override IEventAggregator OnCreateEventAggregator()
        {
            return this.Container.Resolve<IEventAggregator>();
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <param name="fromType">The interface type to register.</param>
        /// <param name="toType">The type implementing the interface.</param>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        protected void RegisterTypeIfMissing(Type fromType, Type toType, bool registerAsSingleton)
        {
            if (fromType == null)
            {
                throw new ArgumentNullException(nameof(fromType));
            }

            if (toType == null)
            {
                throw new ArgumentNullException(nameof(toType));
            }

            if (this.Container.IsTypeRegistered(fromType))
            {
                this.Logger.Log(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Type {0} already registered with container",
                        fromType.Name), 
                    Category.Debug, 
                    Priority.Low);
            }
            else
            {
                if (registerAsSingleton)
                {
                    this.Container.RegisterType(fromType, toType, new ContainerControlledLifetimeManager());
                }
                else
                {
                    this.Container.RegisterType(fromType, toType);
                }
            }
        }

        /// <summary>
        /// Finds all the assemblies in the package. 
        /// Ignoring "System", "Microsoft" and "Prism" assemblies.
        /// </summary>
        /// <returns>List of assemblies found.</returns>
        protected virtual async Task<List<Assembly>> GetAssembliesAsync()
        {
            var assemblyList = new List<Assembly>();

            var folder = Package.Current.InstalledLocation;

            foreach (var file in await folder.GetFilesAsync())
            {
                if ((file.FileType == ".dll" || file.FileType == ".exe") && 
                    !file.DisplayName.StartsWith("System") && 
                    !file.DisplayName.StartsWith("Microsoft") && 
                    !file.DisplayName.StartsWith("Prism"))
                {
                    var assemblyName = new AssemblyName(file.DisplayName);

                    try
                    {
                        var assembly = Assembly.Load(assemblyName);

                        assemblyList.Add(assembly);
                    }
                    catch (Exception)
                    {
                        // Ignore all the exceptions while trying to load a Assembly.
                    }
                }
            }

            return assemblyList;
        }
    }
}