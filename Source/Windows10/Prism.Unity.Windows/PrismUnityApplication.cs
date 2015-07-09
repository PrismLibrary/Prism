using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Windows;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Prism.Unity.Windows
{
    /// <summary>
    /// Provides the base class for the Windows Store Application object which
    /// includes the automatic creation and wiring of the Unity container and 
    /// the bootstrapping process for Prism services in the container.
    /// </summary>
    public abstract class PrismUnityApplication : PrismApplication, IDisposable
    {
        #region Constructor
        public PrismUnityApplication()
        {
            Logger = CreateLogger();
            if (Logger == null)
            {
                throw new InvalidOperationException("Logger Facade is null");
            }

            Logger.Log("Created Logger", Category.Debug, Priority.Low);

            Container = CreateContainer();
            if (Container == null)
            {
                throw new InvalidOperationException("Unity container is null");
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Allow strongly typed access to the Application as a global
        /// </summary>
        public static new PrismUnityApplication Current => (PrismUnityApplication) Application.Current;

        /// <summary>
        /// Get the IoC Unity Container 
        /// </summary>
        public IUnityContainer Container { get; private set; }

        /// <summary>
        /// Gets the <see cref="ILoggerFacade"/> for the application.
        /// </summary>
        /// <value>A <see cref="ILoggerFacade"/> instance.</value>
        protected ILoggerFacade Logger { get; set; }
        #endregion

        #region Overrides
        /// <summary>
        /// Implements and seals the OnInitialize method to configure the container.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            ConfigureContainer();
            ConfigureViewModelLocator();

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Implements and seals the Resolves method to be handled by the Unity Container.
        /// Use the container to resolve types (e.g. ViewModels and Flyouts)
        /// so their dependencies get injected
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A concrete instance of the specified type.</returns>
        protected override sealed object Resolve(Type type)
        {
            return Container.Resolve(type);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Create the <see cref="ILoggerFacade" /> used by the bootstrapper.
        /// </summary>
        /// <remarks>
        /// The base implementation returns a new DebugLogger.
        /// </remarks>
        protected virtual ILoggerFacade CreateLogger()
        {
            return new DebugLogger();
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
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => Container.Resolve(type));
        }

        protected virtual void ConfigureContainer()
        {
            // Register the unity container with itself so that it can be dependency injected
            // for programmatic registration and resolving of types
            Container.RegisterInstance(Container);

            // Set up the global locator service for any Prism framework code that needs DI 
            // without being coupled to Unity
            Logger.Log("Setting up ServiceLocator", Category.Debug, Priority.Low);
            var serviceLocator = new UnityServiceLocator(Container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            Container.RegisterInstance<IServiceLocator>(serviceLocator);

            Logger.Log("Adding UnityExtensions to container", Category.Debug, Priority.Low);
            Container.AddNewExtension<PrismUnityExtension>();

            Logger.Log("Registering Prism services with container", Category.Debug, Priority.Low);
            Container.RegisterInstance<ILoggerFacade>(Logger);
            RegisterTypeIfMissing(typeof(IEventAggregator), typeof(EventAggregator), true);
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
                throw new ArgumentNullException("fromType");
            }
            if (toType == null)
            {
                throw new ArgumentNullException("toType");
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
        public void Dispose()
        {
            if (Container != null)
            {
                Container.Dispose();
                Container = null;
            }
        }
        #endregion


    }
}