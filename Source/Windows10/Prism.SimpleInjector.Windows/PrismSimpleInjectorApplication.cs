using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Microsoft.Practices.ServiceLocation;
using Prism.Logging;
using Prism.Windows;
using SimpleInjector;

namespace Prism.SimpleInjector.Windows
{
    /// <summary>
    /// Provides the base class for the Universal Windows Platform application object which
    /// includes the automatic creation and wiring of the Simple Injector container and
    /// the bootstrapping process for Prism services in the container.
    /// </summary>
    public abstract class PrismSimpleInjectorApplication : PrismApplication, IDisposable
    {
        /// <summary>
        /// Allow strongly typed access to the Application as a global
        /// </summary>
        public new static PrismSimpleInjectorApplication Current => (PrismSimpleInjectorApplication)Application.Current;

        /// <summary>
        /// Get the IoC Simple Injector Container
        /// </summary>
        public Container Container { get; private set; }

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
        /// Implements the Resolves method to be handled by the Simple Injector Container.
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
        /// Creates the <see cref="Container" /> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="Container" />.</returns>
        protected virtual Container CreateContainer()
        {
            return new Container();
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
            Container.RegisterSingleton(Logger);
        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator" />.
        /// </summary>
        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => new SimpleInjectorServiceLocatorAdapter(Container));
        }

        /// <summary>
        /// Override this method with the initialization logic of your application. Here you can initialize services, repositories, and so on.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs" /> instance containing the event data.</param>
        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Container.RegisterSingleton(SessionStateService);
            Container.RegisterSingleton(DeviceGestureService);
            Container.RegisterSingleton(NavigationService);
            Container.RegisterSingleton(EventAggregator);

            return Task.CompletedTask;
        }
    }
}