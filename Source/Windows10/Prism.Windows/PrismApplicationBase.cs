using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Prism
{
    public abstract class PrismApplicationBase : Application, IPrismApplicationEvents
    {
        static int _initialized = 0;
        public new static PrismApplicationBase Current => (PrismApplicationBase)Application.Current;
        private static readonly SemaphoreSlim _startSemaphore = new SemaphoreSlim(1, 1);
        public const string NavigationServiceParameterName = "navigationService";

        public PrismApplicationBase()
        {
            InternalInitialize();

            base.Suspending += async (s, e) =>
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Suspend_Data"))
                {
                    ApplicationData.Current.LocalSettings.Values.Remove("Suspend_Data");
                }
                ApplicationData.Current.LocalSettings.Values.Add("Suspend_Data", DateTime.Now.ToString());
                var deferral = e.SuspendingOperation.GetDeferral();
                try
                {
                    OnSuspending();
                    await OnSuspendingAsync();
                }
                finally
                {
                    deferral.Complete();
                }
            };
            base.Resuming += async (s, e) =>
            {
                await InternalStartAsync(new StartArgs(ResumeArgs.Create(ApplicationExecutionState.Suspended), StartKinds.Resume));
            };
        }

        private IContainerExtension _containerExtension;
        public IContainerProvider Container => _containerExtension;

        protected INavigationService NavigationService { get; private set; }

        private void InternalInitialize()
        {
            // dependecy injection
            _containerExtension = CreateContainerExtension();
            RegisterRequiredTypes(_containerExtension);

            RegisterTypes(_containerExtension);

            _containerExtension.FinalizeExtension();

            // finalize the application
            ConfigureViewModelLocator();

            ConfigureModuleCatalog(Container.Resolve<IModuleCatalog>());
            InitializeModules();
        }


        private void CallOnInitializedOnce()
        {
            // once and only once, ever
            if (Interlocked.Increment(ref _initialized) == 1)
            {
                NavigationService = CreateNavigationService(null, null, SupportedNavigationGestures());

                OnInitialized();
            }
        }

        private async Task InternalStartAsync(StartArgs startArgs)
        {
            await _startSemaphore.WaitAsync();

            try
            {
                CallOnInitializedOnce();
                TestResuming(startArgs);
                OnStart(startArgs);
                await OnStartAsync(startArgs);
                Window.Current.Activate();
            }
            catch (Exception ex)
            {
                Container.Resolve<ILoggerFacade>().Log(ex.ToString(), Category.Exception, Priority.High);
            }
            finally
            {
                _startSemaphore.Release();
            }
        }

        private static void TestResuming(StartArgs startArgs)
        {
            if (startArgs.Arguments is ILaunchActivatedEventArgs e
                && e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Suspend_Data"))
                {
                    ApplicationData.Current.LocalSettings.Values.Remove("Suspend_Data");
                    startArgs.Arguments = ResumeArgs.Create(ApplicationExecutionState.Terminated);
                    startArgs.StartKind = StartKinds.Resume;
                }
            }
        }

        protected virtual void OnSuspending() { /* empty */ }

        protected virtual Task OnSuspendingAsync() => Task.CompletedTask;

        protected abstract void RegisterTypes(IContainerRegistry containerRegistry);

        protected virtual void OnInitialized()
        {
            NavigationService.SetAsWindowContent(Window.Current, true);
        }

        protected virtual void OnStart(StartArgs args) {  /* empty */ }

        protected virtual Task OnStartAsync(StartArgs args) => Task.CompletedTask;

        protected virtual Gesture[] SupportedNavigationGestures() => new Gesture[] { Gesture.Back, Gesture.Forward, Gesture.Refresh };

        protected virtual void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, viewModelType) =>
            {
                INavigationService navigationService = null;

                if (view is Page page && page.Frame != null)
                {
                    navigationService = CreateNavigationService(page.Frame, null, SupportedNavigationGestures());
                }

                return Container.Resolve(viewModelType, (typeof(INavigationService), navigationService));
            });
        }

        protected virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) { /* empty */ }

        protected virtual void InitializeModules()
        {
            if (Container.Resolve<IModuleCatalog>().Modules.Any())
            {
                IModuleManager manager = Container.Resolve<IModuleManager>();
                manager.Run();
            }
        }

        protected abstract IContainerExtension CreateContainerExtension();

        protected virtual void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IPlatformNavigationService, NavigationService>(NavigationServiceParameterName);
            containerRegistry.Register<IGestureService, GestureService>();
            containerRegistry.Register<IFrameFacade, FrameFacade>();

            // standard prism services
            containerRegistry.RegisterInstance<IContainerExtension>(_containerExtension);
            containerRegistry.RegisterSingleton<ILoggerFacade, DebugLogger>();
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.RegisterSingleton<IModuleCatalog, ModuleCatalog>();
            containerRegistry.RegisterSingleton<IModuleManager, ModuleManager>();
            containerRegistry.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
        }

        #region Factory Methods

        private INavigationService CreateNavigationService(Frame frame, CoreWindow window, params Gesture[] gestures)
        {
            if (frame is null)
                frame = new Frame();

            if (window is null)
                window = Window.Current.CoreWindow;

            var gesture_service = CreateGestureServiceForWindow(window);
            var navigation_service = Container.Resolve<IPlatformNavigationService>(NavigationServiceParameterName, (typeof(Frame), frame));
            foreach (var gesture in gestures)
            {
                switch (gesture)
                {
                    case Gesture.Back:
                        gesture_service.BackRequested += async (s, e) => await navigation_service.GoBackAsync();
                        break;
                    case Gesture.Forward:
                        gesture_service.ForwardRequested += async (s, e) => await navigation_service.GoForwardAsync(default(INavigationParameters));
                        break;
                    case Gesture.Refresh:
                        gesture_service.RefreshRequested += async (s, e) => await navigation_service.RefreshAsync();
                        break;
                }
            }

            return navigation_service;
        }

        IGestureService CreateGestureServiceForWindow(CoreWindow window)
        {
            var service = Container.Resolve<IGestureService>((typeof(CoreWindow), window));

            // remove when closed
            void Window_Closed(CoreWindow sender, CoreWindowEventArgs args)
            {
                window.Closed -= Window_Closed;
                if (service is IDestructibleGestureService disposable)
                {
                    disposable.Destroy(window);
                }
            }
            window.Closed += Window_Closed;

            return service;
        }

        #endregion

        #region Sealed Application Methods

        protected override sealed async void OnActivated(IActivatedEventArgs e) => await InternalStartAsync(new StartArgs(e, StartKinds.Activate));
        protected override sealed async void OnCachedFileUpdaterActivated(CachedFileUpdaterActivatedEventArgs e) => await InternalStartAsync(new StartArgs(e, StartKinds.Activate));
        protected override sealed async void OnFileActivated(FileActivatedEventArgs e) => await InternalStartAsync(new StartArgs(e, StartKinds.Activate));
        protected override sealed async void OnFileOpenPickerActivated(FileOpenPickerActivatedEventArgs e) => await InternalStartAsync(new StartArgs(e, StartKinds.Activate));
        protected override sealed async void OnFileSavePickerActivated(FileSavePickerActivatedEventArgs e) => await InternalStartAsync(new StartArgs(e, StartKinds.Activate));
        protected override sealed async void OnSearchActivated(SearchActivatedEventArgs e) => await InternalStartAsync(new StartArgs(e, StartKinds.Activate));
        protected override sealed async void OnShareTargetActivated(ShareTargetActivatedEventArgs e) => await InternalStartAsync(new StartArgs(e, StartKinds.Activate));
        protected override sealed async void OnLaunched(LaunchActivatedEventArgs e) => await InternalStartAsync(new StartArgs(e, StartKinds.Launch));
        protected override sealed async void OnBackgroundActivated(BackgroundActivatedEventArgs e) => await InternalStartAsync(new StartArgs(e, StartKinds.Background));

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);
            _windowCreated?.Invoke(this, args);
        }

#endregion

#region Prism Events

#pragma warning disable CS0067 // unused events
        [EditorBrowsable(EditorBrowsableState.Never)]
        private new event EventHandler<object> Resuming;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private new event SuspendingEventHandler Suspending;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private new event EnteredBackgroundEventHandler EnteredBackground;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private new event LeavingBackgroundEventHandler LeavingBackground;

#pragma warning restore CS0067

        EnteredBackgroundEventHandler _enteredBackground;
        event EnteredBackgroundEventHandler IPrismApplicationEvents.EnteredBackground
        {
            add { _enteredBackground += value; }
            remove { _enteredBackground -= value; }
        }

        LeavingBackgroundEventHandler _leavingBackground;
        event LeavingBackgroundEventHandler IPrismApplicationEvents.LeavingBackground
        {
            add { _leavingBackground += value; }
            remove { _leavingBackground -= value; }
        }

        TypedEventHandler<PrismApplicationBase, WindowCreatedEventArgs> _windowCreated;
        event TypedEventHandler<PrismApplicationBase, WindowCreatedEventArgs> IPrismApplicationEvents.WindowCreated
        {
            add { _windowCreated += value; }
            remove { _windowCreated -= value; }
        }

#endregion
    }
}
