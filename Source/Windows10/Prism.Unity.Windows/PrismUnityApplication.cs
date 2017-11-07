using System;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Unity;
using Prism.Events;
using Prism.Logging;
using Prism.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;
using Windows.UI.Xaml;
using Unity.Lifetime;

namespace Prism.Unity.Windows
{
    /// <summary>
    /// Provides the base class for the Universal Windows Platform application object which
    /// includes the automatic creation and wiring of the Unity container and 
    /// the bootstrapping process for Prism services in the container.
    /// </summary>
    public abstract class PrismUnityApplication : PrismApplication, IDisposable
    {
        #region Properties
        /// <summary>
        /// Allow strongly typed access to the Application as a global
        /// </summary>
        public static new PrismUnityApplication Current => (PrismUnityApplication)Application.Current;

        /// <summary>
        /// Get the IoC Unity Container 
        /// </summary>
        public IUnityContainer Container { get; private set; }

        #endregion

        #region Overrides

        protected override void CreateAndConfigureContainer()
        {
            Logger.Log("Creating Container", Category.Debug, Priority.Low);
            Container = CreateContainer();
            if (Container == null)
            {
                throw new InvalidOperationException("Unity container is null");
            }
            Logger.Log("Configuring Container", Category.Debug, Priority.Low);
            ConfigureContainer();
            Logger.Log("Configuring ServiceLocator", Category.Debug, Priority.Low);
            ConfigureServiceLocator();
        }
        /// <summary>
        /// Implements the Resolves method to be handled by the Unity Container.
        /// Use the container to resolve types (e.g. ViewModels and Flyouts)
        /// so their dependencies get injected
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A concrete instance of the specified type.</returns>
        protected override object Resolve(Type type)
        {
            return Container.Resolve(type);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Creates the <see cref="IUnityContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="IUnityContainer"/>.</returns>
        protected virtual IUnityContainer CreateContainer()
        {
            return new UnityContainer();
        }

        /// <summary>
        /// Creates and configures the container and service locator
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            // Register the unity container with itself so that it can be dependency injected
            // for programmatic registration and resolving of types
            Container.RegisterInstance(Container);

            // Set up the global locator service for any Prism framework code that needs DI 
            // without being coupled to Unity
            Logger.Log("Setting up ServiceLocator", Category.Debug, Priority.Low);
            RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);

            Logger.Log("Adding UnityExtensions to container", Category.Debug, Priority.Low);
            Container.AddNewExtension<PrismUnityExtension>();

            Logger.Log("Registering Prism services with container", Category.Debug, Priority.Low);
            Container.RegisterInstance<ILoggerFacade>(Logger);
            RegisterTypeIfMissing(typeof(ISessionStateService), typeof(SessionStateService), true);
            RegisterTypeIfMissing(typeof(IDeviceGestureService), typeof(DeviceGestureService), true);
            RegisterTypeIfMissing(typeof(IEventAggregator), typeof(EventAggregator), true);
        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator" />.
        /// </summary>
        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => Container.Resolve<IServiceLocator>());
        }

        /// <summary>
        /// Creates the nav service through the base class and registers it with the container
        /// </summary>
        /// <param name="rootFrame">The frame where nav happens</param>
        /// <param name="sessionStateService">The session state service</param>
        /// <returns>NavigationService</returns>
        protected override INavigationService CreateNavigationService(IFrameFacade rootFrame, ISessionStateService sessionStateService)
        {
            var svc = base.CreateNavigationService(rootFrame, sessionStateService);
            Container.RegisterInstance<INavigationService>(svc);
            return svc;
        }

        /// <summary>
        /// Creates the SessionStateService as a singleton through the container
        /// </summary>
        /// <returns>The SessionStateService</returns>
        protected override ISessionStateService OnCreateSessionStateService()
        {
            return Container.Resolve<ISessionStateService>();
        }

        /// <summary>
        /// Creates the DeviceGestureService as a singleton through the container
        /// </summary>
        /// <returns>DeviceGestureService instance</returns>
        protected override IDeviceGestureService OnCreateDeviceGestureService()
        {
            var svc = Container.Resolve<IDeviceGestureService>();
            svc.UseTitleBarBackButton = true;
            return svc;
        }

        /// <summary>
        /// Creates the IEventAggregator as a singleton through the container
        /// </summary>
        /// <returns>IEventAggregator instance</returns>
        protected override IEventAggregator OnCreateEventAggregator()
        {
            return Container.Resolve<IEventAggregator>();
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
        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Container != null)
            {
                Container.Dispose();
                Container = null;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}