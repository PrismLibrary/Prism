using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using Prism.Navigation;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Prism.Services;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Prism
{
    

    public abstract partial class PrismApplicationBase : IPrismApplicationBase
    {
        public new static PrismApplicationBase Current => (PrismApplicationBase)Application.Current;
        private static SemaphoreSlim _startSemaphore = new SemaphoreSlim(1, 1);
        public const string NavigationServiceName = "PageNavigationService";

        public PrismApplicationBase()
        {
            InternalInitialize();
            (this as IPrismApplicationEvents).WindowCreated += (s, e) =>
            {
                GestureService.SetupForCurrentView(e.Window.CoreWindow);
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

        private void InternalInitialize()
        {
            // don't forget there is no logger yet
#if UAP10_0_15063
            Debug.WriteLine($"{nameof(PrismApplicationBase)}.{nameof(InternalInitialize)}");
#else
            Console.WriteLine($"{nameof(PrismApplicationBase)}.{nameof(InternalInitialize)}");
#endif


            // dependecy injection
            _containerExtension = CreateContainer();
            RegisterRequiredTypes(_containerExtension as IContainerRegistry);
            RegisterTypes(_containerExtension as IContainerRegistry);
            _containerExtension.FinalizeExtension();

            // finalize the application
            ConfigureViewModelLocator();
            OnInitialized();
        }


        private async Task InternalStartAsync(StartArgs startArgs)
        {
            await _startSemaphore.WaitAsync();
#if UAP10_0_15063
            Debug.WriteLine($"{nameof(PrismApplicationBase)}.{nameof(InternalStartAsync)}({startArgs})");
#else
            Console.WriteLine($"{nameof(PrismApplicationBase)}.{nameof(InternalStartAsync)}({startArgs})");
#endif

            try
            {
                TestResuming(startArgs);
                OnStart(startArgs);
                await OnStartAsync(startArgs);
                Window.Current.Activate();
            }
            catch (Exception ex)
            {
#if UAP10_0_15063
                Debug.WriteLine($"ERROR {ex.Message}");
#else
                Console.WriteLine($"ERROR {ex.Message}");
#endif
            }
            finally
            {
                _startSemaphore.Release();
            }
        }

        private static void TestResuming(StartArgs startArgs)
        {
            if (startArgs.Arguments is ILaunchActivatedEventArgs e && e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Suspend_Data"))
                {
                    startArgs.Arguments = ResumeArgs.Create(ApplicationExecutionState.Terminated);
                    startArgs.StartKind = StartKinds.Resume;
                }
            }
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

        public virtual IContainerExtension CreateContainer()
        {
            return new DefaultContainerExtension();
        }

        protected virtual void RegisterRequiredTypes(IContainerRegistry container)
        {
            // required for view-models

            container.Register<INavigationService, NavigationService>(NavigationServiceName);

            // standard prism services

            container.RegisterSingleton<ILoggerFacade, EmptyLogger>();
            container.RegisterSingleton<IEventAggregator, EventAggregator>();
        }

#endregion
    }
}
