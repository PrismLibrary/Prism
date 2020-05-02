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
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Prism.Modularity;
using HelloWorld.ViewModels;
using Prism.Regions;

namespace HelloUnoWorld
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : PrismApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            ConfigureFilters(global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);

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

        /// <summary>
        /// Configures global logging
        /// </summary>
        /// <param name="factory"></param>
        static void ConfigureFilters(ILoggerFactory factory)
        {
            factory
                .WithFilter(new FilterLoggerSettings
                    {
                        { "Uno", LogLevel.Warning },
                        { "Windows", LogLevel.Warning },

                        // Debug JS interop
                        // { "Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug },

                        // Generic Xaml events
                        // { "Windows.UI.Xaml", LogLevel.Debug },
                        // { "Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug },
                        // { "Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug },
                        // { "Windows.UI.Xaml.UIElement", LogLevel.Debug },

                        // Layouter specific messages
                        // { "Windows.UI.Xaml.Controls", LogLevel.Debug },
                        // { "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
                        // { "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
                        // { "Windows.Storage", LogLevel.Debug },

                        // Binding related messages
                        // { "Windows.UI.Xaml.Data", LogLevel.Debug },

                        // DependencyObject memory references tracking
                        // { "ReferenceHolder", LogLevel.Debug },
                    }
                )
#if DEBUG
                .AddConsole(LogLevel.Debug);
#else
                .AddConsole(LogLevel.Information);
#endif
        }


        protected override UIElement CreateShell()
        {
            return Container.Resolve<Shell>();
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
