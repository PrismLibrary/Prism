using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Windows;

namespace Prism.Unity.Windows
{
    /// <summary>
    /// Provides the base class for the Windows Store Application object which
    /// includes the automatic creation and wiring of the Unity container.
    /// </summary>
    public abstract class PrismUnityApplication : PrismApplication
    {
        /// <summary>
		/// Get the IoC Unity Container 
		/// </summary>
        public IUnityContainer Container { get; } = new UnityContainer();

        /// <summary>
		/// Creates a new instance of PrismUnityApplication.
		/// </summary>
        public PrismUnityApplication()
        {
            // Register the unity container
            Container.RegisterInstance(Container);

            // Set up the global locator service
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(Container));

            // Allow the implementation class the opportunity to register
            // types early in the process. Do not allow exceptions to abort
            // the object creation.
            try
            {
                OnEarlyContainerRegistration(Container);
            }
            catch (Exception ex)
            {
                OnUnhandledRegistrationException(ex);
            }
        }

        public static new PrismUnityApplication Current => (PrismUnityApplication)Application.Current;


        /// <summary>
        /// Implements and seals the OnInitialize method. The implementation
        /// of this method calls a new OnApplcationInitialize method.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        protected override sealed void OnInitialize(IActivatedEventArgs args)
        {
            try
            {
                // Allow the implementing class the opportunity to
                // register types. Do not allow exceptions to abort
                // the initialization process.
                try
                {
                    OnContainerRegistration(Container);
                }
                catch (Exception ex)
                {
                    OnUnhandledRegistrationException(ex);
                }

                // Set the ViewModel Locator service to use the Unity Container
                ViewModelLocationProvider.SetDefaultViewModelFactory(viewModelType => ServiceLocator.Current.GetInstance(viewModelType));
            }
            finally
            {
                OnApplicationInitialize(args);
            }
        }

        /// <summary>
        /// Override this method with the initialization logic of your application. Here you can initialize 
        /// services, repositories, and so on.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnApplicationInitialize(IActivatedEventArgs args)
        {
        }

        /// <summary>
        /// Implements and seals the Resolves method to be handled by the Unity Container.
        /// Use the container to resolve types (e.g. ViewModels and Flyouts)
        /// so their dependencies get injected
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A concrete instance of the specified type.</returns>
        protected override sealed object Resolve(Type type) => ServiceLocator.Current.GetInstance<IUnityContainer>().Resolve(type);

        /// <summary>
        /// Override this method with code to initialize your container. This method will contain calls
        /// to the Unity container's RegisterType and RegisterInstance methods for example.
        /// </summary>
        /// <param name="container">The instance of the unity container that should be used for registering types.</param>
        protected virtual void OnContainerRegistration(IUnityContainer container)
        {
        }

        /// <summary>
        /// Override this method to register types in the container prior to any other code
        /// being run. This is especially useful when types need to be registered for application
        /// session state to be restored. Certain types may not be available or should not be registered
        /// in this method. For example, registering the Pub/Sub 
        /// </summary>
        /// <param name="container">The instance of the unity container that should be used for registering types.</param>
        protected virtual void OnEarlyContainerRegistration(IUnityContainer container)
        {
        }

        /// <summary>
        /// Override this method to catch any unhandled exceptions that occur during the registration process.
        /// </summary>
        /// <param name="ex">The exception that was thrown.</param>
        protected virtual void OnUnhandledRegistrationException(Exception ex)
        {
        }
    }
}