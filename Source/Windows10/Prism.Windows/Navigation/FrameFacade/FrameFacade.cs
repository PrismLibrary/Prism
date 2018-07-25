using Prism.Navigation;
using Prism.Ioc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using Prism.Utilities;
using System.Threading;
using Windows.UI.Xaml;

namespace Prism.Navigation
{
    public class FrameFacadeProps : DependencyObject
    {
        public static string GetCurrentNavigationPath(Frame obj)
            => (string)obj.GetValue(CurrentNavigationPathProperty);
        public static void SetCurrentNavigationPath(Frame obj, string value)
            => obj.SetValue(CurrentNavigationPathProperty, value);
        public static readonly DependencyProperty CurrentNavigationPathProperty =
            DependencyProperty.RegisterAttached("CurrentNavigationPath", typeof(string),
                typeof(FrameFacadeProps), new PropertyMetadata(string.Empty));
    }

    public class FrameFacade : IFrameFacade, IFrameFacade2
    {
        private readonly Frame _frame;
        private readonly CoreDispatcher _dispatcher;
        private readonly SynchronizationContext _syncContext;
        private readonly IPlatformNavigationService _navigationService;
        public event EventHandler CanGoBackChanged;
        public event EventHandler CanGoForwardChanged;

        internal FrameFacade(Frame frame, IPlatformNavigationService navigationService)
        {
            _frame = frame;
            _frame.ContentTransitions = new TransitionCollection();
            _frame.ContentTransitions.Add(new NavigationThemeTransition());
            _frame.RegisterPropertyChangedCallback(Frame.CanGoBackProperty, (s, p)
                => CanGoBackChanged?.Invoke(this, EventArgs.Empty));
            _frame.RegisterPropertyChangedCallback(Frame.CanGoForwardProperty, (s, p)
                => CanGoForwardChanged?.Invoke(this, EventArgs.Empty));

            _dispatcher = frame.Dispatcher;
            _syncContext = SynchronizationContext.Current;
            _navigationService = navigationService;
        }

        Frame IFrameFacade2.Frame
            => _frame;

        string IFrameFacade2.CurrentNavigationPath
        {
            get => FrameFacadeProps.GetCurrentNavigationPath(_frame);
            set => FrameFacadeProps.SetCurrentNavigationPath(_frame, value);
        }

        public bool CanGoBack()
            => _frame.CanGoBack;

        public bool CanGoForward()
            => _frame.CanGoForward;

        public async Task<INavigationResult> GoBackAsync(INavigationParameters parameters,
            NavigationTransitionInfo infoOverride)
        {
            if (!CanGoBack())
            {
                return this.Failure($"{nameof(CanGoBack)} is false.");
            }

            return await OrchestrateAsync(
              parameters: parameters,
              mode: Prism.Navigation.NavigationMode.Back,
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
            if (!CanGoForward())
            {
                return this.Failure($"{nameof(CanGoForward)} is false.");
            }

            return await OrchestrateAsync(
                parameters: parameters,
                mode: Prism.Navigation.NavigationMode.Forward,
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
            return await OrchestrateAsync(
                parameters: null,
                mode: Prism.Navigation.NavigationMode.Refresh,
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
            Debug.WriteLine($"{nameof(FrameFacade)}.{nameof(NavigateAsync)}({queue})");

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

                if (result.Success)
                {
                    (this as IFrameFacade2).CurrentNavigationPath = pageNavinfo.ToString();
                }
                else
                {
                    return result;
                }
            }

            // finally

            return this.Successful();
        }

        private async Task<INavigationResult> NavigateAsync(
            INavigationPath pageNavInfo,
            NavigationTransitionInfo infoOverride)
        {
            Debug.WriteLine($"{nameof(FrameFacade)}.{nameof(NavigateAsync)}({pageNavInfo})");

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

            var old_vm = (_frame.Content as Page)?.DataContext;
            if (old_vm == null)
            {
                Debug.WriteLine($"[From]View-Model is null.");
            }
            else if (!await CanNavigateAsync(parameters, old_vm))
            {
                return this.Failure($"[From]{old_vm}.CanNavigateAsync returned false.");
            }
            else if (!CanNavigate(parameters, old_vm))
            {
                return this.Failure($"[From]{old_vm}.CanNavigate returned false.");
            }

            // navigate

            var success = await NavigateFrameAsync(navigate);
            Debug.WriteLine($"{nameof(FrameFacade)}.{nameof(OrchestrateAsync)}.NavigateFrameAsync() returned {success}.");
            if (!success)
            {
                return this.Failure("NavigateFrameAsync() returned false.");
            }

            var new_page = _frame.Content as Page;
            if (new_page == null)
            {
                throw new Exception("There is no new page in Frame after Navigate.");
            }

            // post-events

            if (old_vm != null)
            {
                OnNavigatedFrom(parameters, old_vm);
            }

            var new_vm = new_page?.DataContext;
            if (new_vm == null)
            {
                if (Mvvm.ViewModelLocator.GetAutowireViewModel(new_page) == null)
                {
                    // developer didn't set autowire, and did't set datacontext manually

                    // set the autowire & see if we can find it for them
                    Mvvm.ViewModelLocator.SetAutowireViewModel(new_page, true);

                    // TODO: I wonder if I need to delay for a second?
                    new_vm = new_page.DataContext;
                }
            }

            if (new_vm == null)
            {
                Debug.WriteLine($"[To]View-Model is null.");
            }
            else
            {
                await OnNavigatedToAsync(parameters, new_vm);
                OnNavigatingTo(parameters, new_vm);
                OnNavigatedTo(parameters, new_vm);
            }

            // refresh-bindings

            BindingUtilities.UpdateBindings(new_page);

            // finally

            return this.Successful();
        }

        private static async Task<bool> CanNavigateAsync(INavigationParameters parameters, object vm)
        {
            Debug.WriteLine($"Calling {nameof(CanNavigateAsync)}");
            var confirm = true;
            if (vm is IConfirmNavigationAsync old_vm_confirma)
            {
                confirm = await old_vm_confirma.CanNavigateAsync(parameters);
                Debug.WriteLine($"[From]{old_vm_confirma}.{nameof(IConfirmNavigationAsync)} is {confirm}.");
            }
            else
            {
                Debug.WriteLine($"[From]{nameof(IConfirmNavigationAsync)} not implemented.");
            }
            return confirm;
        }

        private static bool CanNavigate(INavigationParameters parameters, object vm)
        {
            Debug.WriteLine($"Calling {nameof(CanNavigate)}");
            var confirm = true;
            if (vm is IConfirmNavigation old_vm_confirms)
            {
                confirm = old_vm_confirms.CanNavigate(parameters);
                Debug.WriteLine($"[From]{old_vm_confirms}.{nameof(IConfirmNavigation)} is {confirm}.");
            }
            else
            {
                Debug.WriteLine($"[From]{nameof(IConfirmNavigation)} not implemented.");
            }
            return confirm;
        }

        private static void OnNavigatedFrom(INavigationParameters parameters, object vm)
        {
            Debug.WriteLine($"Calling {nameof(OnNavigatedFrom)}");
            if (vm != null)
            {
                if (vm is INavigatedAware old_vm_ed)
                {
                    old_vm_ed.OnNavigatedFrom(parameters);
                    Debug.WriteLine($"{nameof(INavigatedAware)}.OnNavigatedFrom() called.");
                }
                else
                {
                    Debug.WriteLine($"{nameof(INavigatedAware)} not implemented.");
                }
            }
        }

        private static void OnNavigatingTo(INavigationParameters parameters, object vm)
        {
            Debug.WriteLine($"Calling {nameof(OnNavigatingTo)}");
            if (vm is INavigatingAware new_vm_ing)
            {
                new_vm_ing.OnNavigatingTo(parameters);
                Debug.WriteLine($"{nameof(INavigatingAware)}.OnNavigatingTo() called.");
            }
            else
            {
                Debug.WriteLine($"{nameof(INavigatingAware)} not implemented.");
            }
        }

        private static async Task OnNavigatedToAsync(INavigationParameters parameters, object vm)
        {
            Debug.WriteLine($"Calling {nameof(OnNavigatedToAsync)}");
            if (vm is INavigatedAwareAsync new_vm_ed)
            {
                await new_vm_ed.OnNavigatedToAsync(parameters);
                Debug.WriteLine($"{nameof(INavigatedAwareAsync)}.OnNavigatedToAsync() called.");
            }
            else
            {
                Debug.WriteLine($"{nameof(INavigatedAwareAsync)} not implemented.");
            }
        }

        private static void OnNavigatedTo(INavigationParameters parameters, object vm)
        {
            Debug.WriteLine($"Calling {nameof(OnNavigatedTo)}");
            if (vm is INavigatedAware new_vm_ed)
            {
                new_vm_ed.OnNavigatedTo(parameters);
                Debug.WriteLine($"{nameof(INavigatedAware)}.OnNavigatedTo() called.");
            }
            else
            {
                Debug.WriteLine($"{nameof(INavigatedAware)} not implemented.");
            }
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
            Debug.WriteLine($"{nameof(FrameFacade)}.{nameof(NavigateFrameAsync)} HasThreadAccess: {_dispatcher.HasThreadAccess}");

            void failedHandler(object s, NavigationFailedEventArgs e)
            {
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
                throw new Exception("Exception in FrameFacade.NavigateFrameAsync().", ex);
            }
            finally
            {
                _frame.NavigationFailed -= failedHandler;
            }
        }
    }
}
