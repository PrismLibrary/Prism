using System;
using System.Threading;
using System.Threading.Tasks;
using Prism.Logging;
using Prism.Utilities;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Prism.Navigation
{
    public class FrameFacade : IFrameFacade, IFrameProvider
    {
        private readonly bool _logStartingEvents = false;

        private readonly Frame _frame;
        private readonly CoreDispatcher _dispatcher;
        private readonly SynchronizationContext _syncContext;
        private readonly ILoggerFacade _logger;

        public event EventHandler CanGoBackChanged;
        public event EventHandler CanGoForwardChanged;

        public FrameFacade(Frame frame, ILoggerFacade logger)
        {
            _frame = frame;
            _frame.ContentTransitions = new TransitionCollection
            {
                new NavigationThemeTransition()
            };
            _frame.RegisterPropertyChangedCallback(Frame.CanGoBackProperty, (s, p)
                => CanGoBackChanged?.Invoke(this, EventArgs.Empty));
            _frame.RegisterPropertyChangedCallback(Frame.CanGoForwardProperty, (s, p)
                => CanGoForwardChanged?.Invoke(this, EventArgs.Empty));

            _dispatcher = frame.Dispatcher;
            _syncContext = SynchronizationContext.Current;
            _logger = logger;
        }

        Frame IFrameProvider.Frame
            => _frame;

        public bool CanGoBack()
            => _frame.CanGoBack;

        public bool CanGoForward()
            => _frame.CanGoForward;

        public async Task<INavigationResult> GoBackAsync(INavigationParameters parameters,
            NavigationTransitionInfo infoOverride)
        {
            _logger.Log("FrameFacade.GoBackAsync()", Category.Info, Priority.Low);

            if (!CanGoBack())
            {
                return this.NavigationFailure($"{nameof(CanGoBack)} is false; exiting GoBackAsync().");
            }

            return await OrchestrateAsync(
              parameters: parameters,
              mode: NavigationMode.Back,
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
              navigate: async () =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
              {
                  _frame.GoBack(infoOverride);
                  return true;
              });
        }

        public async Task<INavigationResult> GoForwardAsync(INavigationParameters parameters)
        {
            _logger.Log("FrameFacade.GoForwardAsync()", Category.Info, Priority.Low);

            if (!CanGoForward())
            {
                return this.NavigationFailure($"{nameof(CanGoForward)} is false.");
            }

            return await OrchestrateAsync(
                parameters: parameters,
                mode: NavigationMode.Forward,
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                navigate: async () =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                {
                    _frame.GoForward();
                    return true;
                });
        }

        public INavigationParameters CurrentParameters { get; private set; }

        public async Task<INavigationResult> RefreshAsync()
        {
            _logger.Log("FrameFacade.RefreshAsync()", Category.Info, Priority.Low);

            var original = _frame.BackStackDepth;
            var state = _frame.GetNavigationState();

            return await OrchestrateAsync(
                parameters: CurrentParameters,
                mode: NavigationMode.Refresh,
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                navigate: async () =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                {
                    _frame.SetNavigationState(state);
                    return Equals(_frame.BackStackDepth, original);
                });
        }

        async Task<INavigationResult> NavigateAsync(
            string path,
            INavigationParameters parameter,
            NavigationTransitionInfo infoOverride)
        {
            return await NavigateAsync(
                queue: NavigationQueue.Parse(path, parameter),
                infoOverride: infoOverride);
        }

        public async Task<INavigationResult> NavigateAsync(
            Uri path,
            INavigationParameters parameter,
            NavigationTransitionInfo infoOverride)
        {
            return await NavigateAsync(
                queue: NavigationQueue.Parse(path, parameter),
                infoOverride: infoOverride);
        }

        private async Task<INavigationResult> NavigateAsync(
            NavigationQueue queue,
            NavigationTransitionInfo infoOverride)
        {
            _logger.Log($"{nameof(FrameFacade)}.{nameof(NavigateAsync)}({queue})", Category.Info, Priority.None);

            // clear stack, if requested

            if (queue.ClearBackStack)
            {
                _frame.SetNavigationState(new Frame().GetNavigationState());
            }

            // iterate through queue

            while (queue.Count > 0)
            {
                var pageNavinfo = queue.Dequeue();

                var result = await NavigateAsync(
                    pageNavInfo: pageNavinfo,
                    infoOverride: infoOverride);

                // do not continue on failure

                if (!result.Success)
                {
                    return result;
                }
            }

            // finally

            return this.NavigationSuccess();
        }

        private async Task<INavigationResult> NavigateAsync(
            INavigationPath pageNavInfo,
            NavigationTransitionInfo infoOverride)
        {
            _logger.Log($"{nameof(FrameFacade)}.{nameof(NavigateAsync)}({pageNavInfo})", Category.Info, Priority.Low);

            return await OrchestrateAsync(
                parameters: pageNavInfo.Parameters,
                mode: NavigationMode.New,
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                navigate: async () =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                {
                    /*
                     * To enable serialization of the frame's state using GetNavigationState, 
                     * you must pass only basic types to this method, such as string, char, 
                     * numeric, and GUID types. If you pass an object as a parameter, an entry 
                     * in the frame's navigation stack holds a reference on the object until the 
                     * frame is released or that entry is released upon a new navigation that 
                     * diverges from the stack. In general, we discourage passing a non-basic 
                     * type as a parameter to Navigate because it can’t be serialized when the 
                     * application is suspended, and can consume more memory because a reference 
                     * is held by the frame’s navigation stack to allow the application to go forward and back. 
                     * https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.Frame#Windows_UI_Xaml_Controls_Frame_Navigate_Windows_UI_Xaml_Interop_TypeName_System_Object_
                     */
                    var parameter = pageNavInfo.QueryString;
                    return _frame.Navigate(
                      sourcePageType: pageNavInfo.View,
                      parameter: parameter,
                      infoOverride: infoOverride);
                });
        }

        private async Task<INavigationResult> OrchestrateAsync(
            INavigationParameters parameters,
            Prism.Navigation.NavigationMode mode,
            Func<Task<bool>> navigate)
        {
            // setup default parameters

            CurrentParameters = parameters;
            parameters = UpdateInternalParameters(parameters, mode);

            // pre-events

            var old_vm = (_frame.Content as Page)?.DataContext;
            if (old_vm is null)
            {
                _logger.Log($"No view-model is set for source page; this is okay; skipping all the [from] overrides including CanNavigate/Async.", Category.Info, Priority.None);
            }
            else if (!await CanNavigateAsync(parameters, old_vm))
            {
                return this.NavigationFailure($"[From]{old_vm}.CanNavigateAsync returned false; this is okay; FrameFacade orchestration will stop here.");
            }
            else if (!CanNavigate(parameters, old_vm))
            {
                return this.NavigationFailure($"[From]{old_vm}.CanNavigate returned false; this is okay; FrameFacade orchestration will stop here.");
            }

            // navigate

            var success = await NavigateFrameAsync(navigate);
            _logger.Log($"{nameof(FrameFacade)}.{nameof(OrchestrateAsync)}.NavigateFrameAsync() returned {success}.", Category.Info, Priority.None);
            if (!success)
            {
                return this.NavigationFailure("NavigateFrameAsync() returned false; this is very unusual, but possibly okay; FrameFacade orchestration will stop here.");
            }

            if (!(_frame.Content is Page new_page))
            {
                var message = "There is no new page in FrameFacade after NavigateFrameAsync; this is a critical failure. Check the page constructor, maybe?";
                _logger.Log(message, Category.Exception, Priority.High);
                throw new Exception(message);
            }

            // post-events

            if (old_vm != null)
            {
                OnNavigatedFrom(parameters, old_vm);
            }

            var new_vm = new_page?.DataContext;
            if (new_vm is null)
            {
                if (Mvvm.ViewModelLocator.GetAutowireViewModel(new_page) is null)
                {
                    // developer didn't set autowire, and did't set datacontext manually
                    _logger.Log("No view-model is set for target page, we will attempt to find view-model declared using RegisterForNavigation<P, VM>().", Category.Info, Priority.None);

                    // set the autowire & see if we can find it for them
                    Mvvm.ViewModelLocator.SetAutowireViewModel(new_page, true);

                    // TODO: I wonder if I need to delay for a second?

                    new_vm = new_page.DataContext;

                    if (new_vm != null)
                    {
                        _logger.Log($"View-Model: {new_vm} found for target View: {new_page}.", Category.Info, Priority.None);
                    }
                }
            }

            if (new_vm is null)
            {
                _logger.Log($"View-Model for source page not found; this is okay, skipping all the [to] overides.", Category.Info, Priority.None);
            }
            else
            {
                OnNavigatingTo(parameters, new_vm);
                await OnNavigatedToAsync(parameters, new_vm);
                OnNavigatedTo(parameters, new_vm);
            }

            // refresh-bindings

            BindingUtilities.UpdateBindings(new_page);

            // finally

            return this.NavigationSuccess();
        }

        private async Task<bool> CanNavigateAsync(INavigationParameters parameters, object vm)
        {
            if (_logStartingEvents)
            {
                _logger.Log($"STARTING {nameof(CanNavigateAsync)} parameters:{parameters}", Category.Info, Priority.None);
            }

            var confirm = true;
            if (vm is IConfirmNavigationAsync old_vm_confirma)
            {
                confirm = await old_vm_confirma.CanNavigateAsync(parameters);
                _logger.Log($"[From]{old_vm_confirma}.{nameof(IConfirmNavigationAsync)} returned {confirm}.", Category.Info, Priority.None);
            }
            else
            {
                _logger.Log($"[From]{nameof(IConfirmNavigationAsync)} not implemented; this is okay; we'll move on to next step in FrameFacade orchestration.", Category.Info, Priority.None);
            }
            return confirm;
        }

        private bool CanNavigate(INavigationParameters parameters, object vm)
        {
            if (_logStartingEvents)
            {
                _logger.Log($"STARTING {nameof(CanNavigate)} parameters:{parameters}", Category.Info, Priority.None);
            }

            var confirm = true;
            if (vm is IConfirmNavigation old_vm_confirms)
            {
                confirm = old_vm_confirms.CanNavigate(parameters);
                _logger.Log($"[From]{old_vm_confirms}.{nameof(IConfirmNavigation)} is {confirm}.", Category.Info, Priority.None);
            }
            else
            {
                _logger.Log($"[From]{nameof(IConfirmNavigation)} not implemented; this is okay; we'll move on to next step in FrameFacade orchestration.", Category.Info, Priority.None);
            }
            return confirm;
        }

        private void OnNavigatedFrom(INavigationParameters parameters, object vm)
        {
            if (_logStartingEvents)
            {
                _logger.Log($"STARTING {nameof(OnNavigatedFrom)} parameters:{parameters}", Category.Info, Priority.None);
            }

            if (vm != null)
            {
                if (vm is INavigatedAware old_vm_ed)
                {
                    old_vm_ed.OnNavigatedFrom(parameters);
                    _logger.Log($"{nameof(INavigatedAware)}.OnNavigatedFrom() called.", Category.Info, Priority.None);
                }
                else
                {
                    _logger.Log($"{nameof(INavigatedAware)} not implemented; this is okay; we'll move on to next step in FrameFacade orchestration.", Category.Info, Priority.None);
                }
            }
        }

        private void OnNavigatingTo(INavigationParameters parameters, object vm)
        {
            if (_logStartingEvents)
            {
                _logger.Log($"STARTING {nameof(OnNavigatingTo)} parameters:{parameters}", Category.Info, Priority.None);
            }

            if (vm is INavigatingAware new_vm_ing)
            {
                new_vm_ing.OnNavigatingTo(parameters);
                _logger.Log($"{nameof(INavigatingAware)}.OnNavigatingTo() called.", Category.Info, Priority.None);
            }
            else
            {
                _logger.Log($"{nameof(INavigatingAware)} not implemented; this is okay; we'll move on to next step in FrameFacade orchestration.", Category.Info, Priority.None);
            }
        }

        private async Task OnNavigatedToAsync(INavigationParameters parameters, object vm)
        {
            if (_logStartingEvents)
            {
                _logger.Log($"STARTING {nameof(OnNavigatedToAsync)} parameters:{parameters}", Category.Info, Priority.None);
            }

            if (vm is INavigatedAwareAsync new_vm_ed)
            {
                await new_vm_ed.OnNavigatedToAsync(parameters);
                _logger.Log($"{nameof(INavigatedAwareAsync)}.OnNavigatedToAsync() called.", Category.Info, Priority.None);
            }
            else
            {
                _logger.Log($"{nameof(INavigatedAwareAsync)} not implemented; this is okay; we'll move on to next step in FrameFacade orchestration.", Category.Info, Priority.None);
            }
        }

        private void OnNavigatedTo(INavigationParameters parameters, object vm)
        {
            if (_logStartingEvents)
            {
                _logger.Log($"STARTING {nameof(OnNavigatedTo)} parameters:{parameters}", Category.Info, Priority.None);
            }

            if (vm is INavigatedAware new_vm_ed)
            {
                new_vm_ed.OnNavigatedTo(parameters);
                _logger.Log($"{nameof(INavigatedAware)}.OnNavigatedTo() called.", Category.Info, Priority.None);
            }
            else
            {
                _logger.Log($"{nameof(INavigatedAware)} not implemented; this is okay; we'll move on to next step in FrameFacade orchestration.", Category.Info, Priority.None);
            }
        }

        private INavigationParameters UpdateInternalParameters(INavigationParameters parameters, Prism.Navigation.NavigationMode mode)
        {
            parameters = parameters ?? new NavigationParameters();
            parameters.SetNavigationMode(mode);
            parameters.SetSyncronizationContext(_syncContext);
            return parameters;
        }

        private async Task<bool> NavigateFrameAsync(Func<Task<bool>> navigate)
        {
            if (_logStartingEvents)
            {
                _logger.Log($"STARTING {nameof(FrameFacade)}.{nameof(NavigateFrameAsync)} HasThreadAccess: {_dispatcher.HasThreadAccess}", Category.Info, Priority.None);
            }

            void failedHandler(object s, NavigationFailedEventArgs e)
            {
                _logger.Log($"Frame.NavigationFailed raised. {e.SourcePageType}:{e.Exception.Message}", Category.Exception, Priority.High);
                throw e.Exception;
            }

            try
            {
                _frame.NavigationFailed += failedHandler;

                if (_dispatcher.HasThreadAccess)
                {
                    return await navigate();
                }
                else
                {
                    var result = false;
                    await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        result = await navigate();
                    });
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Exception in FrameFacade.NavigateFrameAsync() {ex}", Category.Exception, Priority.None);
                throw new Exception("Exception in FrameFacade.NavigateFrameAsync().", ex);
            }
            finally
            {
                _frame.NavigationFailed -= failedHandler;
            }
        }
    }
}
