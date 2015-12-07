using System;
using Windows.UI.Xaml;
using LightInject;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Logging;
using Prism.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;

namespace Prism.LightInject.Windows
{
    /// <summary>
    /// Provides the base class for the Universal Windows Platform application object which
    /// includes the automatic creation and wiring of the LightInjector container and
    /// the bootstrapping process for Prism services in the container.
    /// </summary>
    public abstract class PrismLightInjectApplication : PrismApplication, IDisposable
    {
        /// <summary>
        /// Allow strongly typed access to the Application as a global
        /// </summary>
        public new static PrismLightInjectApplication Current => (PrismLightInjectApplication)Application.Current;

        /// <summary>
        /// Get the IoC LightInjector Container
        /// </summary>
        public IServiceContainer Container { get; private set; }

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

        /// <summary>
        /// Implements the Resolves method to be handled by the LightInjector Container.
        /// Use the container to resolve types (e.g. ViewModels and Flyouts)
        /// so their dependencies get injected
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A concrete instance of the specified type.</returns>
        protected override object Resolve(Type type)
        {
            return Container.GetInstance(type);
        }

        /// <summary>
        /// Creates the <see cref="IServiceContainer" /> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="IServiceContainer" />.</returns>
        protected virtual IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }

        /// <summary>
        /// Creates and configures the container and service locator
        /// </summary>
        protected override void CreateAndConfigureContainer()
        {
            Logger.Log("Creating and Configuring Container", Category.Debug, Priority.Low);
            Container = CreateContainer();
            if (Container == null)
            {
                throw new InvalidOperationException("Simple Injector container is null");
            }

            Logger.Log("Configuring Container", Category.Debug, Priority.Low);
            ConfigureContainer();
            Logger.Log("Configuring ServiceLocator", Category.Debug, Priority.Low);
            ConfigureServiceLocator();
        }

        /// <summary>
        /// Configures the container
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            Logger.Log("Registering Prism services with container", Category.Debug, Priority.Low);
            Container.RegisterInstance(typeof(ILoggerFacade), Logger);
            Container.Register<ISessionStateService, SessionStateService>(new PerContainerLifetime());
            Container.Register<IDeviceGestureService, DeviceGestureService>(new PerContainerLifetime());
            Container.Register<IEventAggregator, EventAggregator>(new PerContainerLifetime());
        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator" />.
        /// </summary>
        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => Container.GetInstance<IServiceLocator>());
        }

        /// <summary>
        /// Creates the nav service through the base class and gets it registered with the container
        /// </summary>
        /// <param name="rootFrame">The frame where nav happens</param>
        /// <param name="sessionStateService">The session state service that stores nav state on suspend.</param>
        /// <returns>The NavigationService instance</returns>
        protected override INavigationService CreateNavigationService(IFrameFacade rootFrame, ISessionStateService sessionStateService)
        {
            var navigationService = base.CreateNavigationService(rootFrame, sessionStateService);
            Container.RegisterInstance(typeof(INavigationService), navigationService);
            return navigationService;
        }

        /// <summary>
        /// Creates the DeviceGestureService through the container
        /// </summary>
        /// <returns>DeviceGestureService</returns>
        protected override IDeviceGestureService OnCreateDeviceGestureService()
        {
            var deviceGestureService = Container.GetInstance<IDeviceGestureService>();
            deviceGestureService.UseTitleBarBackButton = true;
            return deviceGestureService;
        }

        /// <summary>
        /// Creates the IEventAggregator through the container
        /// </summary>
        /// <returns>IEventAggregator</returns>
        protected override IEventAggregator OnCreateEventAggregator()
        {
            return Container.GetInstance<IEventAggregator>();
        }

        /// <summary>
        /// Creates the SessionStateService through the container
        /// </summary>
        /// <returns>SessionStateService</returns>
        protected override ISessionStateService OnCreateSessionStateService()
        {
            return Container.GetInstance<ISessionStateService>();
        }
    }
}