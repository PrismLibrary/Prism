namespace Prism.Composition.Windows
{
    using System;
    using System.Globalization;
    using Events;
    using Extensions;
    using Logging;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Prism.Windows;
    using Prism.Windows.AppModel;
    using Prism.Windows.Navigation;
    using global::Windows.UI.Xaml;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Windows.ApplicationModel;
    using System.Reflection;
    
    /// <summary>A prism composition application.</summary>
    public abstract class PrismCompositionApplication : PrismApplication, IDisposable
    {
        /// <summary>The current.</summary>
        public static new PrismCompositionApplication Current => (PrismCompositionApplication)Application.Current;
        
        /// <summary>Gets the container.</summary>
        /// <value>The container.</value>
        public IUnityContainer Container { get; private set; }
        
        /// <summary>Creates and Configures the container if using a container.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid. </exception>
        protected override async void CreateAndConfigureContainer()
        {
            Logger.Log("Creating Container", Category.Debug, Priority.Low);
            Container = CreateContainer();
            if (Container == null)
            {
                throw new InvalidOperationException("Unity container is null");
            }
            Logger.Log("Configuring Container", Category.Debug, Priority.Low);
            await ConfigureContainer();
            Logger.Log("Configuring ServiceLocator", Category.Debug, Priority.Low);
            ConfigureServiceLocator();
        }
        
        /// <summary>Resolves the specified type.</summary>
        /// <param name="type">The type. </param>
        /// <returns>A concrete instance of the specified type.</returns>
        protected override object Resolve(Type type)
        {
            return Container.Resolve(type);
        }
        
        /// <summary>Creates the container.</summary>
        /// <returns>The new container.</returns>
        protected virtual IUnityContainer CreateContainer()
        {
            return new UnityContainer();
        }
        
        /// <summary>Gets or sets the assembly configurations.</summary>
        /// <value>The assembly configurations.</value>
        protected virtual IEnumerable<AssemblyConfiguration> AssemblyConfigurations { get; set; } = new AssemblyConfiguration[] { };
        
        /// <summary>Configure container.</summary>
        /// <returns>A Task.</returns>
        protected virtual async Task ConfigureContainer()
        {
            Container.RegisterInstance(Container);

            Logger.Log("Setting up ServiceLocator", Category.Debug, Priority.Low);
            RegisterTypeIfMissing(typeof(IServiceLocator), typeof(ServiceLocatorAdapter), true);


            Logger.Log("Registering Prism services with container", Category.Debug, Priority.Low);
            Container.RegisterInstance<ILoggerFacade>(Logger);
            RegisterTypeIfMissing(typeof(ISessionStateService), typeof(SessionStateService), true);
            RegisterTypeIfMissing(typeof(IDeviceGestureService), typeof(DeviceGestureService), true);
            RegisterTypeIfMissing(typeof(IEventAggregator), typeof(EventAggregator), true);

            Logger.Log("Adding CompositionExtensions to container", Category.Debug, Priority.Low);
            if (this.AssemblyConfigurations.Count() == 0)
            {
                this.AssemblyConfigurations = await this.CreateAssemblyConfigurations();
            }

            this.Container.RegisterAssemblyConfiguration(this.AssemblyConfigurations);
        }
        
        /// <summary>Creates assembly configurations.</summary>
        /// <returns>The new assembly configurations.</returns>
        private async Task<IEnumerable<AssemblyConfiguration>> CreateAssemblyConfigurations()
        {
            var folder = Package.Current.InstalledLocation;

            var assemblyConfigurations = new List<AssemblyConfiguration>();

            foreach (var file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".dll" && !(file.DisplayName.StartsWith("System") || file.DisplayName.StartsWith("Microsoft") || file.DisplayName.StartsWith("Prism")))
                {
                    var assemblyName = new AssemblyName(file.DisplayName);

                    try
                    {
                        var assembly = Assembly.Load(assemblyName);

                        assemblyConfigurations.Add(new AssemblyConfiguration { Assembly = assembly });
                    }
                    catch (Exception) { }
                }
            }

            assemblyConfigurations.Add(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            return assemblyConfigurations;
        }
        
        /// <summary>Configures the LocatorProvider for the <see cref="T:Microsoft.Practices.ServiceLocation.ServiceLocator" />.</summary>
        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => Container.Resolve<IServiceLocator>());
        }
        
        /// <summary>Creates the navigation service.</summary>
        /// <param name="rootFrame">          The root frame. </param>
        /// <param name="sessionStateService">The session state service. </param>
        /// <returns>The initialized navigation service.</returns>
        protected override INavigationService CreateNavigationService(IFrameFacade rootFrame, ISessionStateService sessionStateService)
        {
            var svc = base.CreateNavigationService(rootFrame, sessionStateService);
            Container.RegisterInstance<INavigationService>(svc);
            return svc;
        }
        
        /// <summary>Creates the session state service. Use this to inject your own ISessionStateService implementation.</summary>
        /// <returns>The initialized session state service.</returns>
        protected override ISessionStateService OnCreateSessionStateService()
        {
            return Container.Resolve<ISessionStateService>();
        }
        
        /// <summary>Creates the device gesture service. Use this to inject your own IDeviceGestureService implementation.</summary>
        /// <returns>The initialized device gesture service.</returns>
        protected override IDeviceGestureService OnCreateDeviceGestureService()
        {
            var svc = Container.Resolve<IDeviceGestureService>();
            svc.UseTitleBarBackButton = true;
            return svc;
        }
        
        /// <summary>Create the <see cref="T:Prism.Events.IEventAggregator" /> used for Prism framework events. Use this to inject your own IEventAggregator implementation.</summary>
        /// <returns>The initialized EventAggregator.</returns>
        protected override IEventAggregator OnCreateEventAggregator()
        {
            return Container.Resolve<IEventAggregator>();
        }
        
        /// <summary>Registers the type if missing.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null. </exception>
        /// <param name="fromType">           Type of from. </param>
        /// <param name="toType">             Type of to. </param>
        /// <param name="registerAsSingleton">true to register as singleton. </param>
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
            if (Container.IsTypeRegistered(fromType))
            {
                Logger.Log(
                    String.Format(CultureInfo.CurrentCulture,
                                  "Type {0} already registered with container",
                                  fromType.Name), Category.Debug, Priority.Low);
            }
            else
            {
                if (registerAsSingleton)
                {
                    Container.RegisterType(fromType, toType, new ContainerControlledLifetimeManager());
                }
                else
                {
                    Container.RegisterType(fromType, toType);
                }
            }
        }
        
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            if (Container != null)
            {
                Container.Dispose();
                Container = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
