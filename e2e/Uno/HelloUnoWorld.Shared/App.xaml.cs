using HelloUnoWorld.Views;
using HelloUnoWorld.Dialogs;
using Microsoft.Extensions.Logging;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Prism.Modularity;
using HelloWorld.ViewModels;
using Prism.Regions;

#if HAS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace HelloUnoWorld
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : PrismApplication
    {
        private static Window _window;

#if HAS_UNO_WINUI || NETCOREAPP
        public static XamlRoot MainXamlRoot { get; private set; }
#endif

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeLogging();

            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
#if __IOS__
            // requires Xamarin Test Cloud Agent
            Xamarin.Calabash.Start();
#endif

            base.OnLaunched(args);
        }

#if !HAS_UNO_WINUI && !NETCOREAPP
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        protected override void OnSuspending(SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
#endif

        /// <summary>
        /// Configures global Uno Platform logging
        /// </summary>
        private static void InitializeLogging()
        {
            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                    builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                    builder.AddDebug();
#else
                    builder.AddConsole();
#endif

                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                // builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

                // Layouter specific messages
                // builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

                // builder.AddFilter("Windows.Storage", LogLevel.Debug );

                // Binding related messages
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

                // Binder memory references tracking
                // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

                // RemoteControl and HotReload related
                // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
            });

            global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;
        }

        protected override UIElement CreateShell()
        {
            var shell = Container.Resolve<Shell>();

#if NET5_0 && WINDOWS
            _window = new Window();
            _window.Activate();
            _window.Content = shell;
#endif

#if HAS_UNO_WINUI || NETCOREAPP
            MainXamlRoot = shell.XamlRoot;
#endif

            return shell;
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleA.ModuleAModule>(InitializationMode.OnDemand);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate("InitialRegion", nameof(InitialView));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>(nameof(ViewA));
            containerRegistry.RegisterForNavigation<InitialView>(nameof(InitialView));
            containerRegistry.RegisterForNavigation<ModulesPage, ModulesPageViewModel>();

            containerRegistry.RegisterDialog<NotificationDialog, NotificationDialogViewModel>();
            containerRegistry.RegisterDialog<ConfirmationDialog, ConfirmationDialogViewModel>();

            //register a custom window host
            containerRegistry.RegisterDialogWindow<CustomContentDialog>();
        }

    }
}
