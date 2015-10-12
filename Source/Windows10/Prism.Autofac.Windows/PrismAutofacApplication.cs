using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Autofac;
using Autofac.Features.ResolveAnything;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;

namespace Prism.Autofac.Windows
{
    /// <summary>
    /// Provides the base class for the Windows Store Application object which
    /// includes the automatic creation and wiring of the Autofac container and 
    /// the bootstrapping process for Prism services in the container.
    /// </summary>
    public abstract class PrismAutofacApplication : PrismApplication, IDisposable
    {
        protected PrismAutofacApplication()
        {
            Logger = CreateLogger();
            if (Logger == null)
            {
                throw new InvalidOperationException("Logger Facade is null");
            }

            Logger.Log("Created Logger", Category.Debug, Priority.Low);
        }

        /// <summary>
        /// Allow strongly typed access to the Application as a global
        /// </summary>
        public static new PrismAutofacApplication Current => (PrismAutofacApplication)Application.Current;

        /// <summary>
        /// Get the IoC Autofac Container 
        /// </summary>
        public IContainer Container { get; private set; }

        /// <summary>
        /// Gets the <see cref="ILoggerFacade"/> for the application.
        /// </summary>
        /// <value>A <see cref="ILoggerFacade"/> instance.</value>
        protected ILoggerFacade Logger { get; set; }

        /// <summary>
        /// Implements and seals the OnInitialize method to configure the container.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        protected sealed override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            CreateAndConfigureContainer();
            ConfigureViewModelLocator();

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Implements and seals the Resolves method to be handled by the Autofac Container.
        /// Use the container to resolve types (e.g. ViewModels and Flyouts) so their dependencies get injected
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A concrete instance of the specified type.</returns>
        protected override sealed object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

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
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => Container.Resolve(type));
        }

        /// <summary>
        /// Creates the <see cref="ContainerBuilder"/> that will be used to create the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="ContainerBuilder"/>.</returns>
        protected virtual ContainerBuilder CreateContainerBuilder()
        {
            return new ContainerBuilder();
        }

        /// <summary>
        /// Creates the <see cref="IContainer"/> that will be used as the default container.
        /// For optimal performance, configuration should be completed before creating the container.
        /// </summary>
        /// <returns>A new instance of <see cref="IContainer"/>.</returns>
        protected virtual IContainer CreateContainer(ContainerBuilder containerBuilder)
        {
            return containerBuilder.Build();
        }

        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
            Logger.Log("Registering Prism services with container", Category.Debug, Priority.Low);
            builder.RegisterInstance<ILoggerFacade>(Logger);
            builder.RegisterInstance<ISessionStateService>(SessionStateService);
            builder.RegisterInstance<INavigationService>(NavigationService);
            builder.RegisterInstance<IDeviceGestureService>(DeviceGestureService);

            RegisterTypeIfMissing<IEventAggregator, EventAggregator>(builder, true);
        }

        /// <summary>
        /// Creates and configures the Autofac container
        /// </summary>
        private void CreateAndConfigureContainer()
        {
            ContainerBuilder builder = CreateContainerBuilder();
            // Make sure any not specifically registered concrete type can resolve.
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            ConfigureContainer(builder);

            Container = CreateContainer(builder);
            if (Container == null)
            {
                throw new InvalidOperationException("Autofac container is null");
            }

            ConfigureServiceLocator();
        }

        private void ConfigureServiceLocator()
        {
            var serviceLocator = new AutofacServiceLocatorAdapter(Container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            RegisterInstance(serviceLocator, typeof (IServiceLocator), registerAsSingleton: true);
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <typeparam name="TFrom">The interface type to register.</typeparam>
        /// <typeparam name="TTarget">The type implementing the interface.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> instance.</param>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        protected void RegisterTypeIfMissing<TFrom, TTarget>(ContainerBuilder builder, bool registerAsSingleton = false)
        {
            if (Container != null && Container.IsRegistered<TFrom>())
            {
                Logger.Log(String.Format(CultureInfo.CurrentCulture, "Type {0} already registered with container", typeof(TFrom).Name),
                    Category.Debug, Priority.Low);
            }
            else
            {
                if (registerAsSingleton)
                {
                    builder.RegisterType<TTarget>().As<TFrom>().SingleInstance();
                }
                else
                {
                    builder.RegisterType<TTarget>().As<TFrom>();
                }
            }
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered,
        /// after the container is already created.
        /// Uses a new ContainerBuilder instance to update the Container.
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
            if (Container.IsRegistered(fromType))
            {
                Logger.Log(String.Format(CultureInfo.CurrentCulture,
                                        "Type {0} already registered with container",
                                        fromType.Name), Category.Debug, Priority.Low);
            }
            else
            {
                ContainerBuilder builder = CreateContainerBuilder();
                if (registerAsSingleton)
                {
                    builder.RegisterType(toType).As(fromType).SingleInstance();
                }
                else
                {
                    builder.RegisterType(toType).As(fromType);
                }
                builder.Update(Container);
            }
        }

        /// <summary>
        /// Registers an object instance in the container after the container is already created.
        /// </summary>
        /// <param name="instance">Object instance.</param>
        /// <param name="fromType">The interface type to register.</param>
        /// <param name="key">Optional key for registration.</param>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        protected void RegisterInstance<T>(T instance, Type fromType, string key = "", bool registerAsSingleton = false)
            where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (fromType == null)
            {
                throw new ArgumentNullException(nameof(fromType));
            }

            ContainerBuilder containerUpdater = CreateContainerBuilder();

            var registration = containerUpdater.RegisterInstance(instance);
            // named instance
            if (!string.IsNullOrEmpty(key))
            {
                registration = registration.Named(key, fromType);
            }
            else
            {
                registration = registration.As(fromType);
            }
            // singleton
            if (registerAsSingleton)
            {
                registration.SingleInstance();
            }

            containerUpdater.Update(Container);
        }

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
