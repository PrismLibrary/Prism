using System;
using System.Globalization;
using Windows.UI.Xaml;
using Autofac;
using Autofac.Features.ResolveAnything;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Logging;
using Prism.Windows;
using Prism.Windows.AppModel;
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
        /// <summary>
        /// Allow strongly typed access to the Application as a global
        /// </summary>
        public static new PrismAutofacApplication Current => (PrismAutofacApplication)Application.Current;

        /// <summary>
        /// Get the IoC Autofac Container 
        /// </summary>
        public IContainer Container { get; private set; }

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

        /// <summary>
        /// Configures Prism services in the container
        /// </summary>
        /// <param name="builder">The ContainerBuilder instance that is used to</param>
        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
            Logger.Log("Registering Prism services with container", Category.Debug, Priority.Low);
            builder.RegisterInstance<ILoggerFacade>(Logger);
            RegisterTypeIfMissing<ISessionStateService, SessionStateService>(builder, true);
            RegisterTypeIfMissing<IDeviceGestureService, DeviceGestureService>(builder, true);
            RegisterTypeIfMissing<IEventAggregator, EventAggregator>(builder, true);
        }

        /// <summary>
        /// Creates the nav service through the base class and gets it registered with the container
        /// </summary>
        /// <param name="rootFrame">The frame where nav happens</param>
        /// <param name="sessionStateService">The session state service that stores nav state on suspend.</param>
        /// <returns>The NavigationService instance</returns>
        protected override INavigationService CreateNavigationService(IFrameFacade rootFrame, ISessionStateService sessionStateService)
        {
            var svc = base.CreateNavigationService(rootFrame, sessionStateService);
            RegisterInstance(svc, typeof(INavigationService), registerAsSingleton: true);
            return svc;
        }

        /// <summary>
        /// Creates the DeviceGestureService through the container
        /// </summary>
        /// <returns>DeviceGestureService</returns>
        protected override IDeviceGestureService OnCreateDeviceGestureService()
        {
            var svc = Container.Resolve<IDeviceGestureService>();
            svc.UseTitleBarBackButton = true;
            return svc;
        }

        /// <summary>
        /// Creates the IEventAggregator through the container
        /// </summary>
        /// <returns>IEventAggregator</returns>
        protected override IEventAggregator OnCreateEventAggregator()
        {
            return Container.Resolve<IEventAggregator>();
        }

        /// <summary>
        /// Creates the SessionStateService through the container
        /// </summary>
        /// <returns>SessionStateService</returns>
        protected override ISessionStateService OnCreateSessionStateService()
        {
            return Container.Resolve<ISessionStateService>();
        }

        /// <summary>
        /// Creates and configures the Autofac container
        /// </summary>
        protected override void CreateAndConfigureContainer()
        {
            Logger.Log("Creating and Configuring Container", Category.Debug, Priority.Low);
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

        /// <summary>
        /// Sets up the ServiceLocator to use the Autofac container for any creation of types
        /// </summary>
        protected override void ConfigureServiceLocator()
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
