using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Logging;
using Prism.Services;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Navigation
{
    public class NavigationService : IPlatformNavigationService, IPlatformNavigationService2
    {
        IFrameFacade IPlatformNavigationService2.FrameFacade => _frame;

        public static Dictionary<Frame, INavigationService> Instances { get; } = new Dictionary<Frame, INavigationService>();

        /// <summary>
        /// Creates a navigation service
        /// </summary>
        /// <param name="gestures">Optional default getures tied to this Frame</param>
        /// <returns>INavigationService</returns>
        public static INavigationService Create(params Gesture[] gestures)
        {
            return Create(new Frame(), Window.Current.CoreWindow, gestures);
        }

        /// <summary>
        /// Creates a navigation service
        /// </summary>
        /// <param name="frame">Required XAML Frame</param>
        /// <param name="gestures">Optional default getures tied to this Frame</param>
        /// <returns>INavigationService</returns>
        public static INavigationService Create(Frame frame, params Gesture[] gestures)
        {
            return Create(frame, Window.Current.CoreWindow, gestures);
        }

        /// <summary>
        /// Creates a navigation service
        /// </summary>
        /// <param name="gestures">Optional default getures tied to this Frame</param>
        /// <returns>INavigationService</returns>
        public static INavigationService Create(CoreWindow window, params Gesture[] gestures)
        {
            return Create(new Frame(), window, gestures);
        }

        /// <summary>
        /// Creates a navigation service
        /// </summary>
        /// <param name="frame">Required XAML Frame</param>
        /// <param name="gestures">Optional default getures tied to this Frame</param>
        /// <returns>INavigationService</returns>
        public static INavigationService Create(Frame frame, CoreWindow window, params Gesture[] gestures)
        {
            frame = frame ?? new Frame();
            var gesture_service = GestureService.GetForCurrentView(window);
            var navigation_service = new NavigationService(frame);
            foreach (var gesture in gestures)
            {
                switch (gesture)
                {
                    case Gesture.Back:
                        gesture_service.BackRequested += async (s, e) => await navigation_service.GoBackAsync();
                        break;
                    case Gesture.Forward:
                        gesture_service.ForwardRequested += async (s, e) => await navigation_service.GoForwardAsync();
                        break;
                    case Gesture.Refresh:
                        gesture_service.RefreshRequested += async (s, e) => await navigation_service.RefreshAsync();
                        break;
                }
            }
            return navigation_service;
        }

        /// <summary>
        /// Creates navigation service
        /// </summary>
        /// <param name="frame">Pre-existing frame</param>
        /// <returns>INavigationService</returns>
        public static INavigationService Create(Frame frame)
        {
            return new NavigationService(frame);
        }

        private readonly IFrameFacade _frame;
        private readonly ILoggerFacade _logger;

        private NavigationService(Frame frame)
        {
            _frame = new FrameFacade(frame, this);
            _frame.CanGoBackChanged += (s, e) =>
                CanGoBackChanged?.Invoke(this, EventArgs.Empty);
            _frame.CanGoForwardChanged += (s, e) =>
                CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
            Instances.Add(frame, this);
            _logger = PrismApplicationBase.Current.Container.Resolve<ILoggerFacade>();
        }

        public async Task RefreshAsync()
            => await _frame.RefreshAsync();

        // go forward

        public event EventHandler CanGoForwardChanged;

        public bool CanGoForward()
            => _frame.CanGoForward();

        public async Task<INavigationResult> GoForwardAsync()
            => await GoForwardAsync(
                parameters: default(INavigationParameters));

        public async Task<INavigationResult> GoForwardAsync(INavigationParameters parameters)
        {
            if (parameters == null && (_frame as IFrameFacade2).Frame.ForwardStack.Any())
            {
                var previous = (_frame as IFrameFacade2).Frame.ForwardStack.Last().Parameter?.ToString();
                parameters = new NavigationParameters(previous);
            }

            return await _frame.GoForwardAsync(
                  parameters: parameters);
        }

        // go back

        public event EventHandler CanGoBackChanged;

        public bool CanGoBack()
            => _frame.CanGoBack();

        public async Task<INavigationResult> GoBackAsync()
            => await GoBackAsync(
                parameters: default(INavigationParameters),
                infoOverride: default(NavigationTransitionInfo));

        public async Task<INavigationResult> GoBackAsync(INavigationParameters parameters)
            => await GoBackAsync(
                parameters: parameters,
                infoOverride: default(NavigationTransitionInfo));

        public async Task<INavigationResult> GoBackAsync(INavigationParameters parameters = null, NavigationTransitionInfo infoOverride = null)
        {
            if (parameters == null && (_frame as IFrameFacade2).Frame.BackStack.Any())
            {
                var previous = (_frame as IFrameFacade2).Frame.BackStack.Last().Parameter?.ToString();
                if (previous is null)
                {
                    parameters = new NavigationParameters();
                }
                else
                {
                    parameters = new NavigationParameters(previous);
                }
            }

            return await _frame.GoBackAsync(
                    parameters: parameters,
                    infoOverride: infoOverride);
        }

        // navigate(string)

        public async Task<INavigationResult> NavigateAsync(string path)
            => await NavigateAsync(
                uri: new Uri(path, UriKind.RelativeOrAbsolute),
                parameter: default(INavigationParameters),
                infoOverride: default(NavigationTransitionInfo));

        public async Task<INavigationResult> NavigateAsync(string path, INavigationParameters parameters)
            => await NavigateAsync(
                uri: new Uri(path, UriKind.RelativeOrAbsolute),
                parameter: parameters,
                infoOverride: default(NavigationTransitionInfo));

        public async Task<INavigationResult> NavigateAsync(string path, INavigationParameters parameter, NavigationTransitionInfo infoOverride)
            => await NavigateAsync(
                uri: new Uri(path, UriKind.RelativeOrAbsolute),
                parameter: parameter,
                infoOverride: infoOverride);

        // navigate(uri)

        public async Task<INavigationResult> NavigateAsync(Uri uri)
            => await NavigateAsync(
                uri: uri,
                parameter: default(INavigationParameters),
                infoOverride: default(NavigationTransitionInfo));

        public async Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters)
            => await NavigateAsync(
                uri: uri,
                parameter: parameters,
                infoOverride: default(NavigationTransitionInfo));

        public async Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameter, NavigationTransitionInfo infoOverride)
        {
            _logger.Log($"{nameof(NavigationService)}.{nameof(NavigateAsync)}(uri:{uri} parameter:{parameter} info:{infoOverride})", Category.Info, Priority.None);

            try
            {
                return await _frame.NavigateAsync(
                    uri: uri,
                    parameter: parameter,
                    infoOverride: infoOverride);
            }
            catch (Exception ex)
            {
                _logger.Log($"Navigation error: {ex.Message}", Category.Exception, Priority.High);
                Debugger.Break();
                throw;
            }
        }
    }
}
