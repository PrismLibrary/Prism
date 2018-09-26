## Prism for Windows

This library is a startup orchestration library for UWP applications. The out-of-the box activation of UWP applications requires considerable custom development to support simple, common features standard in this library. For example, the Blank UWP template creates lengthy boilerplate code in app.xaml.cs. What's worse, it is very incomplete. Intended to guide developers, its volume and complexity make most developers want to leave it alone. 

#### Here's the old boilerplate app.xaml.cs created by Visual Studio's Blank app template.

````csharp
sealed partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        this.Suspending += OnSuspending;
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        Frame rootFrame = Window.Current.Content as Frame;

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (rootFrame == null)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            rootFrame.NavigationFailed += OnNavigationFailed;

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            // Place the frame in the current Window
            Window.Current.Content = rootFrame;
        }

        if (e.PrelaunchActivated == false)
        {
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    /// <summary>
    /// Invoked when application execution is being suspended.  Application state is saved
    /// without knowing whether the application will be terminated or resumed with the contents
    /// of memory still intact.
    /// </summary>
    /// <param name="sender">The source of the suspend request.</param>
    /// <param name="e">Details about the suspend request.</param>
    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
        var deferral = e.SuspendingOperation.GetDeferral();
        //TODO: Save application state and stop any background activity
        deferral.Complete();
    }
}
````

This library is a complete rewrite of that logic with several goals in mind: dependecy injection, navigation management, and the introduction of several standard features like event aggregation and view-model support. It makes getting started with UWP considerably more simple, and provides the capabilities most serious developers expect from a platform.

#### Here's the boilerplate app.xaml.cs for Prism.Windows.

````csharp
sealed partial class App : PrismApplication
{
    public static IPlatformNavigationService NavigationService { get; private set; }

    public App()
    {
        InitializeComponent();
    }

    public override void RegisterTypes(IContainerRegistry container)
    {
        container.RegisterForNavigation<MainPage, MainPageViewModel>(nameof(Views.MainPage));
    }

    public override void OnInitialized()
    {
        NavigationService = Prism.Navigation.NavigationService
            .Create(new Frame(), Gestures.Back, Gestures.Forward, Gestures.Refresh);
        NavigationService.SetAsWindowContent(Window.Current, true);
    }

    public override void OnStart(StartArgs args)
    {
        NavigationService.NavigateAsync(nameof(Views.MainPage));
    }
}
````

### Relationship to Xamarin

This library provides a consistent experience for developers using Prism in their Xamarin applications. It shares interfaces, development approaches, and most Prism libraries. That said, it does not require or depend on Xamarin. It simply provides a consistent experience for developers using Prism across platforms.

### Relationship to Template 10

This library used to be [Template 10](http://aka.ms/template10). Template 10 was a startup orchestration library refactored to align with Prism. This includes namespaces and a new startup pipeline accounting for UWP updates. However, Template 10's controls and libraries are not part of this library; they are merged into the Windows Toolkit or remain in Template 10's helper libraries.
