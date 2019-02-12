using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using Prism.Navigation;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Prism.Services;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Prism.Modularity;
using Windows.UI.Xaml.Controls;

namespace Prism
{
    public abstract partial class PrismApplicationBase
    {
        public new static PrismApplicationBase Current => (PrismApplicationBase)Application.Current;
        private static readonly SemaphoreSlim _startSemaphore = new SemaphoreSlim(1, 1);
        public const string NavigationServiceParameterName = "navigationService";
        private readonly bool _logStartingEvents = false;

        public PrismApplicationBase()
        {
            InternalInitialize();
            _logger.Log("[App.Constructor()]", Category.Info, Priority.None);
            (this as IPrismApplicationEvents).WindowCreated += (s, e) =>
            {
                Container.SetupForCurrentView(e.Window.CoreWindow);
            };
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
            // don't forget there is no logger yet
            if (_logStartingEvents)
            {
                _logger.Log($"{nameof(PrismApplicationBase)}.{nameof(InternalInitialize)}", Category.Info, Priority.None);
            }

            // dependecy injection
            _containerExtension = CreateContainerExtension();
            RegisterRequiredTypes(_containerExtension);

            Debug.WriteLine("[App.RegisterTypes()]");
            RegisterTypes(_containerExtension);

            Debug.WriteLine("Dependency container has just been finalized.");
            _containerExtension.FinalizeExtension();

            // now we can start logging instead of debug/write
            _logger = Container.Resolve<ILoggerFacade>();

            // finalize the application
            ConfigureViewModelLocator();

            ConfigureModuleCatalog(Container.Resolve<IModuleCatalog>());
            InitializeModules();
        }

        static int _initialized = 0;
        private ILoggerFacade _logger;

        private void CallOnInitializedOnce()
        {
            // don't forget there is no logger yet
            if (_logStartingEvents)
            {
                _logger.Log($"{nameof(PrismApplicationBase)}.{nameof(CallOnInitializedOnce)}", Category.Info, Priority.None);
            }

            // once and only once, ever
            if (Interlocked.Increment(ref _initialized) == 1)
            {
                NavigationService = Container.CreateNavigationService(SupportedNavigationGestures());

                _logger.Log("[App.OnInitialize()]", Category.Info, Priority.None);
                OnInitialized();
            }
        }

        private async Task InternalStartAsync(StartArgs startArgs)
        {
            await _startSemaphore.WaitAsync();
            if (_logStartingEvents)
            {
                _logger.Log($"{nameof(PrismApplicationBase)}.{nameof(InternalStartAsync)}({startArgs})", Category.Info, Priority.None);
            }

            try
            {
                CallOnInitializedOnce();
                TestResuming(startArgs);
                _logger.Log($"[App.OnStart(startKind:{startArgs.StartKind}, startCause:{startArgs.StartCause})]", Category.Info, Priority.None);
                OnStart(startArgs);
                _logger.Log($"[App.OnStartAsync(startKind:{startArgs.StartKind}, startCause:{startArgs.StartCause})]", Category.Info, Priority.None);
                await OnStartAsync(startArgs);
                Window.Current.Activate();
            }
            catch (Exception ex)
            {
                _logger.Log($"ERROR {ex.Message}", Category.Exception, Priority.High);
                Debugger.Break();
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

        private static DateTime? SuspendData
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue("Suspend_Data", out var value)
                    && value != null
                    && DateTime.TryParse(value.ToString(), out var date))
                {
                    return date;
                }
                else
                {
                    return null;
                }
            }
            set => ApplicationData.Current.LocalSettings.Values["Suspend_Data"] = value;
        }

        #region overrides

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
                    navigationService = Container.CreateNavigationService(page.Frame, SupportedNavigationGestures());
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

#endregion
    }
}
