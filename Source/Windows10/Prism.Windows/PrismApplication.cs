using System;
using System.Globalization;
using System.Threading.Tasks;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Windows.AppModel;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Prism.Events;

namespace Prism.Windows
{
    /// <summary>
    /// Provides the Prism base class for your Universal Windows Platform application.
    /// Takes care of the automatic creation and wiring needed to initialize and start the application.
    /// </summary>
    public abstract class PrismApplication : Application
    {

        private bool _isRestoringFromTermination;

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        protected PrismApplication()
        {
            Suspending += OnSuspending;
            Logger = CreateLogger();
            if (Logger == null)
            {
                throw new InvalidOperationException("Logger Facade is null");
            }

            Logger.Log("Created Logger", Category.Debug, Priority.Low);
        }

        /// <summary>
        /// Gets the <see cref="ILoggerFacade"/> for the application.
        /// </summary>
        /// <value>A <see cref="ILoggerFacade"/> instance.</value>
        protected ILoggerFacade Logger { get; set; }

        /// <summary>
        /// Gets the shell user interface
        /// </summary>
        /// <value>The shell user interface.</value>
        protected UIElement Shell { get; private set; }

        /// <summary>
        /// Gets or sets the session state service.
        /// </summary>
        /// <value>
        /// The session state service.
        /// </value>
        protected ISessionStateService SessionStateService { get; private set; }

        /// <summary>
        /// Gets or sets the navigation service.
        /// </summary>
        /// <value>
        /// The navigation service.
        /// </value>
        protected INavigationService NavigationService { get; private set; }

        /// <summary>
        /// Gets or sets the device gesture service.
        /// </summary>
        /// <value>
        /// The device gesture service.
        /// </value>
        protected IDeviceGestureService DeviceGestureService { get; private set; }

        /// <summary>
        /// Gets the event aggregator that is used to publish Prism framework events.
        /// </summary>
        /// <value>
        /// The Prism framework event aggregator.
        /// </value>
        protected IEventAggregator EventAggregator { get; private set; }

        /// <summary>
        /// Factory for creating the ExtendedSplashScreen instance.
        /// </summary>
        /// <value>
        /// The Func that creates the ExtendedSplashScreen. It requires a SplashScreen parameter,
        /// and must return a Page instance.
        /// </value>
        protected Func<SplashScreen, Page> ExtendedSplashScreenFactory { get; set; }

        /// <summary>
        /// Gets a value indicating whether the application is suspending.
        /// </summary>
        /// <value>
        /// <c>true</c> if the application is suspending; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuspending { get; private set; }

        /// <summary>
        /// Override this method with logic that will be performed after the application is initialized. For example, navigating to the application's home page.
        /// </summary>
        /// <param name="args">The <see cref="LaunchActivatedEventArgs"/> instance containing the event data.</param>
        protected abstract Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args);

        /// <summary>
        /// Override this method with logic that will be performed after the application is activated through other means 
        /// than a normal launch (i.e. Voice Commands, URI activation, being used as a share target from another app).
        ///  For example, navigating to the application's home page.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        protected virtual Task OnActivateApplicationAsync(IActivatedEventArgs args) { return Task.FromResult<object>(null); }

        /// <summary>
        /// Creates and Configures the container if using a container
        /// </summary>
        protected virtual void CreateAndConfigureContainer() { }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="Microsoft.Practices.ServiceLocation.ServiceLocator" />.
        /// </summary>
        protected virtual void ConfigureServiceLocator() { }

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
            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => Resolve(type));
        }

        /// <summary>
        /// Gets the type of the page based on a page token.
        /// </summary>
        /// <param name="pageToken">The page token.</param>
        /// <returns>The type of the page which corresponds to the specified token.</returns>
        protected virtual Type GetPageType(string pageToken)
        {
            var assemblyQualifiedAppType = this.GetType().AssemblyQualifiedName;

            var pageNameWithParameter = assemblyQualifiedAppType.Replace(this.GetType().FullName, this.GetType().Namespace + ".Views.{0}Page");

            var viewFullName = string.Format(CultureInfo.InvariantCulture, pageNameWithParameter, pageToken);
            var viewType = Type.GetType(viewFullName);

            if (viewType == null)
            {
                var resourceLoader = ResourceLoader.GetForCurrentView(Constants.InfrastructureResourceMapId);
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, resourceLoader.GetString("DefaultPageTypeLookupErrorMessage"), pageToken, this.GetType().Namespace + ".Views"),
                    nameof(pageToken));
            }

            return viewType;
        }

        /// <summary>
        /// Used for setting up the list of known types for the SessionStateService, using the RegisterKnownType method.
        /// </summary>
        protected virtual void OnRegisterKnownTypesForSerialization() { }

        /// <summary>
        /// Override this method with the initialization logic of your application. Here you can initialize services, repositories, and so on.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        protected virtual Task OnInitializeAsync(IActivatedEventArgs args)
        {
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A concrete instance of the specified type.</returns>
        protected virtual object Resolve(Type type)
        {
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// OnActivated is the entry point for an application when it is launched via
        /// means other normal user interaction. This includes Voice Commands, URI activation,
        /// being used as a share target from another app, etc.
        /// </summary>
        /// <param name="args">Details about the activation method, including the activation
        /// phrase (for voice commands) and the semantic interpretation, parameters, etc.</param>
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            await InitializeShell(args);

            if (Window.Current.Content != null && (!_isRestoringFromTermination || args != null))
            {
                await OnActivateApplicationAsync(args);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private async Task InitializeShell(IActivatedEventArgs args)
        {
            if (Window.Current.Content == null)
            {
                Frame rootFrame = await InitializeFrameAsync(args);

                Shell = CreateShell(rootFrame);

                Window.Current.Content = Shell ?? rootFrame;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user. Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            await InitializeShell(args);

            // If the app is launched via the app's primary tile, the args.TileId property
            // will have the same value as the AppUserModelId, which is set in the Package.appxmanifest.
            // See http://go.microsoft.com/fwlink/?LinkID=288842
            string tileId = AppManifestHelper.GetApplicationId();

            if (Window.Current.Content != null && (!_isRestoringFromTermination || (args != null && args.TileId != tileId)))
            {
                await OnLaunchApplicationAsync(args);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Create the <see cref="IEventAggregator" /> used for Prism framework events.
        /// </summary>
        /// <returns>The initialized EventAggregator.</returns>
        private IEventAggregator CreateEventAggregator() => OnCreateEventAggregator() ?? new EventAggregator();

        /// <summary>
        /// Create the <see cref="IEventAggregator" /> used for Prism framework events. Use this to inject your own IEventAggregator implementation.
        /// </summary>
        /// <returns>The initialized EventAggregator.</returns>
        protected virtual IEventAggregator OnCreateEventAggregator() => null;

        /// <summary>
        /// Creates the root frame.
        /// </summary>
        /// <returns>The initialized root frame.</returns>
        private Frame CreateRootFrame() => OnCreateRootFrame() ?? new Frame();

        /// <summary>
        /// Creates the root frame. Use this to inject your own Frame implementation.
        /// </summary>
        /// <returns>The initialized root frame.</returns>
        protected virtual Frame OnCreateRootFrame() => null;

        /// <summary>
        /// Creates the session state service.
        /// </summary>
        /// <returns>The initialized session state service.</returns>
        private ISessionStateService CreateSessionStateService() => OnCreateSessionStateService() ?? new SessionStateService();

        /// <summary>
        /// Creates the session state service. Use this to inject your own ISessionStateService implementation.
        /// </summary>
        /// <returns>The initialized session state service.</returns>
        protected virtual ISessionStateService OnCreateSessionStateService() => null;

        /// <summary>
        /// Initializes the Frame and its content.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        /// <returns>A task of a Frame that holds the app content.</returns>
        protected virtual async Task<Frame> InitializeFrameAsync(IActivatedEventArgs args)
        {
            CreateAndConfigureContainer();
            EventAggregator = CreateEventAggregator();

            // Create a Frame to act as the navigation context and navigate to the first page
            var rootFrame = CreateRootFrame();

            if (ExtendedSplashScreenFactory != null)
            {
                Page extendedSplashScreen = this.ExtendedSplashScreenFactory.Invoke(args.SplashScreen);
                rootFrame.Content = extendedSplashScreen;
            }

            var frameFacade = new FrameFacadeAdapter(rootFrame, EventAggregator);

            //Initialize PrismApplication common services
            SessionStateService = CreateSessionStateService();

            //Configure VisualStateAwarePage with the ability to get the session state for its frame
            SessionStateAwarePage.GetSessionStateForFrame =
                frame => SessionStateService.GetSessionStateForFrame(frameFacade);

            //Associate the frame with a key
            SessionStateService.RegisterFrame(frameFacade, "AppFrame");

            NavigationService = CreateNavigationService(frameFacade, SessionStateService);

            DeviceGestureService = CreateDeviceGestureService();
            DeviceGestureService.GoBackRequested += OnGoBackRequested;
            DeviceGestureService.GoForwardRequested += OnGoForwardRequested;

            // Set a factory for the ViewModelLocator to use the default resolution mechanism to construct view models
            Logger.Log("Configuring ViewModelLocator", Category.Debug, Priority.Low);
            ConfigureViewModelLocator();

            OnRegisterKnownTypesForSerialization();
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                await SessionStateService.RestoreSessionStateAsync();
            }

            await OnInitializeAsync(args);

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state and navigate to the last page visited
                try
                {
                    SessionStateService.RestoreFrameState();
                    NavigationService.RestoreSavedNavigation();
                    _isRestoringFromTermination = true;
                }
                catch (SessionStateServiceException)
                {
                    // Something went wrong restoring state.
                    // Assume there is no state and continue
                }
            }

            return rootFrame;
        }

        /// <summary>
        /// Handling the forward navigation request from the <see cref="IDeviceGestureService"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGoForwardRequested(object sender, DeviceGestureEventArgs e)
        {
            if (NavigationService.CanGoForward())
            {
                NavigationService.GoForward();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handling the back navigation request from the <see cref="IDeviceGestureService"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGoBackRequested(object sender, DeviceGestureEventArgs e)
        {
            if (NavigationService.CanGoBack())
            {
                NavigationService.GoBack();
                e.Handled = true;
            }
            else if (DeviceGestureService.IsHardwareBackButtonPresent && e.IsHardwareButton)
            {
                Exit();
            }
        }

        /// <summary>
        /// Creates the device gesture service. Use this to inject your own IDeviceGestureService implementation.
        /// </summary>
        /// <returns>The initialized device gesture service.</returns>
        protected virtual IDeviceGestureService OnCreateDeviceGestureService() => null;

        /// <summary>
        /// Creates the device gesture service.
        /// </summary>
        /// <returns>The initialized device gesture service.</returns>
        private IDeviceGestureService CreateDeviceGestureService()
        {
            var deviceGestureService = OnCreateDeviceGestureService();
            if (deviceGestureService == null)
            {
                deviceGestureService = new DeviceGestureService(EventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
            }

            return deviceGestureService;
        }

        /// <summary>
        /// Creates the navigation service. Use this to inject your own INavigationService implementation.
        /// </summary>
        /// <param name="rootFrame">The root frame.</param>
        /// <returns>The initialized navigation service.</returns>
        protected virtual INavigationService OnCreateNavigationService(IFrameFacade rootFrame) => null;

        /// <summary>
        /// Creates the navigation service.
        /// </summary>
        /// <param name="rootFrame">The root frame.</param>
        /// <param name="sessionStateService">The session state service.</param>
        /// <returns>The initialized navigation service.</returns>
        protected virtual INavigationService CreateNavigationService(IFrameFacade rootFrame, ISessionStateService sessionStateService)
        {
            var navigationService = OnCreateNavigationService(rootFrame) ?? new FrameNavigationService(rootFrame, GetPageType, sessionStateService);
            return navigationService;
        }

        /// <summary>
        /// Creates the shell of the app.
        /// </summary>
        /// <param name="rootFrame"></param>
        /// <returns>The shell of the app.</returns>
        protected virtual UIElement CreateShell(Frame rootFrame)
        {
            return rootFrame;
        }

        /// <summary>
        /// Invoked when application execution is being suspended. Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            IsSuspending = true;
            try
            {
                var deferral = e.SuspendingOperation.GetDeferral();

                //Custom calls before suspending.
                await OnSuspendingApplicationAsync();

                //Bootstrap inform navigation service that app is suspending.
                NavigationService.Suspending();

                // Save application state
                await SessionStateService.SaveAsync();

                deferral.Complete();
            }
            finally
            {
                IsSuspending = false;
            }
        }

        /// <summary>
        /// Invoked when the application is suspending, but before the general suspension calls.
        /// </summary>
        /// <returns>Task to complete.</returns>
        protected virtual Task OnSuspendingApplicationAsync() => Task.FromResult<object>(null);
    }
}
