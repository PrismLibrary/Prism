﻿using Prism.Events;
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
using Windows.UI.Xaml.Controls;
using Prism.Services;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Prism.Modularity;
using Windows.UI.Core;

namespace Prism
{
    public abstract partial class PrismApplicationBase : IPrismApplicationBase
    {
        //public new static PrismApplicationBase Current => (PrismApplicationBase)Application.Current;
        private static SemaphoreSlim _startSemaphore = new SemaphoreSlim(1, 1);
        //public const string NavigationServiceParameterName = "navigationService";

        protected INavigationService NavigationService { get; set; }

        public PrismApplicationBase()
        {
            InternalInitialize();
            _logger.Log("[App.Constructor()]", Category.Info, Priority.None);
            (this as IPrismApplicationEvents).WindowCreated += (s, e) =>
            {
                GestureServiceLocator.GetGestureService(e.Window.CoreWindow);
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

        IContainerExtension _containerExtension;
        public IContainerProvider Container => _containerExtension;

        private void InternalInitialize()
        {
            // dependecy injection
            _containerExtension = CreateContainerExtension();
            RegisterRequiredTypes(_containerExtension as IContainerRegistry);

            Debug.WriteLine("[App.RegisterTypes()]");
            RegisterTypes(_containerExtension as IContainerRegistry);

            Debug.WriteLine("Dependency container has just been finalized.");
            _containerExtension.FinalizeExtension();
            NavigationServiceLocator.RegisterNavigationDelegate(CreateNavigationService);

            // now we can start logging instead of debug/write
            _logger = Container.Resolve<ILoggerFacade>();

            // finalize the application
            ConfigureViewModelLocator();
            GestureServiceLocator.SetDefaultFactory(CreateGestureService);

            ConfigureModuleCatalog(Container.Resolve<IModuleCatalog>());
            InitializeModules();
        }

        static int _initialized = 0;
        private ILoggerFacade _logger;

        private void CallOnInitializedOnce()
        {
            // once and only once, ever
            if (Interlocked.Increment(ref _initialized) == 1)
            {
                NavigationService = CreateNavigationService(null, CoreWindow.GetForCurrentThread());
                _logger.Log("[App.OnInitialize()]", Category.Info, Priority.None);
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

        private INavigationService CreateNavigationService(Frame frame, CoreWindow window)
        {
            var resolver = Container as IDependencyResolver;
            var navigationService = resolver.Resolve<IPlatformNavigationService>(
                (typeof(Frame), frame));

            var gestureService = GestureServiceLocator.GetGestureService(window);
            gestureService.BackRequested += async (s, e) => await navigationService.GoBackAsync();
            gestureService.ForwardRequested += async (s, e) => await navigationService.GoForwardAsync();
            gestureService.RefreshRequested += async (s, e) => await navigationService.RefreshAsync();

            return navigationService;
        }

#region overrides

        public virtual void OnSuspending() { /* empty */ }

        public virtual Task OnSuspendingAsync() => Task.CompletedTask;

        public abstract void RegisterTypes(IContainerRegistry container);

        public virtual void OnInitialized() { /* empty */ }

        public virtual void OnStart(StartArgs args) {  /* empty */ }

        public virtual Task OnStartAsync(StartArgs args) => Task.CompletedTask;

        public virtual void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                return _containerExtension.ResolveViewModelForView(view, type);
            });
        }

        private IGestureService CreateGestureService(CoreWindow window) =>
            ((IDependencyResolver)Container).Resolve<IGestureService>((typeof(CoreWindow), window));

        public virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) { /* empty */ }

        protected void InitializeModules()
        {
            if (Container.Resolve<IModuleCatalog>().Modules.Any())
            {
                if (!_containerExtension.SupportsModules)
                    throw new NotSupportedException("Container does not support the use of Modules.");

                IModuleManager manager = Container.Resolve<IModuleManager>();
                manager.Run();
            }
        }

        public abstract IContainerExtension CreateContainerExtension();

        protected virtual void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            // don't forget there is no logger yet
            Debug.WriteLine($"{nameof(PrismApplicationBase)}.{nameof(RegisterRequiredTypes)}()");

            // required for view-models
            containerRegistry.RegisterInstance<IDependencyResolver>(containerRegistry as IDependencyResolver);
            containerRegistry.Register<IPlatformNavigationService, NavigationService>();

            // standard prism services

            containerRegistry.RegisterSingleton<ILoggerFacade, DebugLogger>();
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.RegisterSingleton<IModuleCatalog, ModuleCatalog>();
            containerRegistry.RegisterSingleton<IModuleManager, ModuleManager>();
            containerRegistry.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
        }

#endregion
    }
}
