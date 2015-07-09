using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Prism.Mvvm;
using Prism.Windows.AppModel;
using Prism.Windows.Interfaces;
using Prism.Windows.Mvvm;

namespace Prism.Windows
{
    public abstract class PrismApplication : Application
    {

        private bool _isRestoringFromTermination;

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        protected PrismApplication()
        {
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Gets or sets the session state service.
        /// </summary>
        /// <value>
        /// The session state service.
        /// </value>
        protected ISessionStateService SessionStateService { get; set; }

        /// <summary>
        /// Gets or sets the navigation service.
        /// </summary>
        /// <value>
        /// The navigation service.
        /// </value>
        protected INavigationService NavigationService { get; set; }

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
                    "pageToken");
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
        /// Gets the Settings charm action items.
        /// </summary>
        /// <returns>The list of Setting charm action items that will populate the Settings pane.</returns>
        protected virtual IList<SettingsCommand> GetSettingsCommands()
        {
            return new List<SettingsCommand>();
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
        /// Invoked when the application is launched normally by the end user. Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var rootFrame = await InitializeFrameAsync(args);

            // If the app is launched via the app's primary tile, the args.TileId property
            // will have the same value as the AppUserModelId, which is set in the Package.appxmanifest.
            // See http://go.microsoft.com/fwlink/?LinkID=288842
            string tileId = AppManifestHelper.GetApplicationId();

            if (rootFrame != null && (!_isRestoringFromTermination || (args != null && args.TileId != tileId)))
            {
                await OnLaunchApplicationAsync(args);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Initializes the Frame and its content.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        /// <returns>A task of a Frame that holds the app content.</returns>
        protected async Task<Frame> InitializeFrameAsync(IActivatedEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (ExtendedSplashScreenFactory != null)
                {
                    Page extendedSplashScreen = this.ExtendedSplashScreenFactory.Invoke(args.SplashScreen);
                    rootFrame.Content = extendedSplashScreen;
                }

                var frameFacade = new FrameFacadeAdapter(rootFrame);

                //Initialize PrismApplication common services
                SessionStateService = new SessionStateService();

                //Configure VisualStateAwarePage with the ability to get the session state for its frame
                VisualStateAwarePage.GetSessionStateForFrame =
                    frame => SessionStateService.GetSessionStateForFrame(frameFacade);

                //Associate the frame with a key
                SessionStateService.RegisterFrame(frameFacade, "AppFrame");

                NavigationService = CreateNavigationService(frameFacade, SessionStateService);

                if (ApiInformation.IsTypePresent("Windows.UI.ApplicationSettings.SettingsPane"))
                {
                    // TODO BL : keep an eye on MSDN for future SDK release updates on SettingsPane
#pragma warning disable CS0618 // Type or member is obsolete // Still marked on MSDN as a valid type for the Windows family
                    SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
#pragma warning restore CS0618
                }
                // Register hardware back button event if present
                if (ApiInformation.IsEventPresent("Windows.Phone.UI.Input.HardwareButtons", "BackPressed"))
                {
                    HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;
                }

                // Set a factory for the ViewModelLocator to use the default resolution mechanism to construct view models
                ViewModelLocationProvider.SetDefaultViewModelFactory(Resolve);

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

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }

        /// <summary>
        /// Creates the navigation service.
        /// </summary>
        /// <param name="rootFrame">The root frame.</param>
        /// <param name="sessionStateService">The session state service.</param>
        /// <returns>The initialized navigation service.</returns>
        private INavigationService CreateNavigationService(IFrameFacade rootFrame, ISessionStateService sessionStateService)
        {
            var navigationService = new FrameNavigationService(rootFrame, GetPageType, sessionStateService);
            return navigationService;
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
        /// Handle hardware back button by navigating back if possible using the NavigationService
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs e)
        {
            if (NavigationService.CanGoBack())
            {
                NavigationService.GoBack();
                e.Handled = true;
            }
            else Exit();
        }

#pragma warning disable CS0618 // Type or member is obsolete  // Still marked on MSDN as a valid type for the Windows family
        /// <summary>
        /// Called when the Settings charm is invoked, this handler populates the Settings charm with the charm items returned by the GetSettingsCommands function.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="SettingsPaneCommandsRequestedEventArgs"/> instance containing the event data.</param>
        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            if (args == null || args.Request == null || args.Request.ApplicationCommands == null)
            {
                return;
            }

            var applicationCommands = args.Request.ApplicationCommands;
            var settingsCommands = GetSettingsCommands();

            foreach (var settingsCommand in settingsCommands)
            {
                applicationCommands.Add(settingsCommand);
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
