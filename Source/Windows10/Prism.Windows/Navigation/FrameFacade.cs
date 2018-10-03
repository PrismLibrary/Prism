using System;
using System.Threading;
using System.Threading.Tasks;
using Prism.Ioc;
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
        private Frame _frame { get; }
        private CoreDispatcher _dispatcher { get; }
        private SynchronizationContext _syncContext { get; }
        private IPlatformNavigationService _navigationService { get; }
        private ILoggerFacade _logger { get; }

        public event EventHandler CanGoBackChanged;
        public event EventHandler CanGoForwardChanged;

        public FrameFacade(Frame frame, IPlatformNavigationService navigationService, ILoggerFacade logger)
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
            _navigationService = navigationService;
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

        public async Task<INavigationResult> RefreshAsync()
        {
            _logger.Log("FrameFacade.RefreshAsync()", Category.Info, Priority.Low);

            return await OrchestrateAsync(
                parameters: null,
                mode: NavigationMode.Refresh,
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                navigate: async () =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                {
                    var original = _frame.BackStackDepth;
                    var state = _frame.GetNavigationState();
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

            parameters = CreateDefaultParameters(parameters, mode);

            // pre-events
            if(!PageUtilities.CanNavigate(_frame.Content, parameters)
                || !await PageUtilities.CanNavigateAsync(_frame.Content, parameters))
            {
                throw new CannotNavigateException($"Cannot navigate from {_frame.Content.GetType().Name}");
            }

            // navigate
            var oldPage = _frame.Content;
            var success = await NavigateFrameAsync(navigate);
            _logger.Log($"{nameof(FrameFacade)}.{nameof(OrchestrateAsync)}.NavigateFrameAsync() returned {success}.", Category.Info, Priority.None);
            if (!success)
            {
                return this.NavigationFailure("NavigateFrameAsync() returned false; this is very unusual, but possibly okay; FrameFacade orchestration will stop here.");
            }

            if (!(_frame.Content is Page newPage))
            {
                var message = "There is no new page in FrameFacade after NavigateFrameAsync; this is a critical failure. Check the page constructor, maybe?";
                _logger.Log(message, Category.Exception, Priority.High);
                throw new Exception(message);
            }

            // post-events

            if(newPage.DataContext is null)
            {
                if (Mvvm.ViewModelLocator.GetAutowireViewModel(newPage) is null)
                {
                    // developer didn't set autowire, and did't set datacontext manually
                    _logger.Log("No view-model is set for target page, we will attempt to find view-model declared using RegisterForNavigation<P, VM>().", Category.Debug, Priority.None);

                    // set the autowire & see if we can find it for them
                    Mvvm.ViewModelLocator.SetAutowireViewModel(newPage, true);
                }
            }

            PageUtilities.OnNavigatingTo(newPage, parameters);
            PageUtilities.OnNavigatedFrom(oldPage, parameters);
            await PageUtilities.OnNavigatedToAsync(newPage, parameters);
            PageUtilities.OnNavigatedTo(newPage, parameters);

            // refresh-bindings

            BindingUtilities.UpdateBindings(newPage);

            // finally

            return this.NavigationSuccess();
        }

        private INavigationParameters CreateDefaultParameters(INavigationParameters parameters, Prism.Navigation.NavigationMode mode)
        {
            parameters = parameters ?? new NavigationParameters();
            parameters.SetNavigationMode(mode);
            parameters.SetNavigationService(_navigationService);
            parameters.SetSyncronizationContext(_syncContext);
            return parameters;
        }

        private async Task<bool> NavigateFrameAsync(Func<Task<bool>> navigate)
        {
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
