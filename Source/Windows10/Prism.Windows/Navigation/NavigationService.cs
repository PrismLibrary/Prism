using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Prism.Logging;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Navigation
{
    public class NavigationService : IPlatformNavigationService, IFrameFacadeProvider
    {
        IFrameFacade IFrameFacadeProvider.FrameFacade => _frameFacade;

        private IFrameFacade _frameFacade { get; }
        private ILoggerFacade _logger { get; }

        public NavigationService(ILoggerFacade logger, IFrameFacade frameFacade)
        {
            _frameFacade = frameFacade;
            _frameFacade.CanGoBackChanged += (s, e) =>
                CanGoBackChanged?.Invoke(this, EventArgs.Empty);
            _frameFacade.CanGoForwardChanged += (s, e) =>
                CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
            _logger = logger;
        }

        public async Task RefreshAsync()
            => await _frameFacade.RefreshAsync();

        // go forward

        public event EventHandler CanGoForwardChanged;

        public bool CanGoForward()
            => _frameFacade.CanGoForward();

        public async Task<INavigationResult> GoForwardAsync()
            => await GoForwardAsync(
                parameters: default(INavigationParameters));

        public async Task<INavigationResult> GoForwardAsync(INavigationParameters parameters)
        {
            if (parameters == null && _frameFacade is IFrameProvider frameProvider && frameProvider.Frame.ForwardStack.Any())
            {
                var previous = frameProvider.Frame.ForwardStack.Last().Parameter?.ToString();
                parameters = new NavigationParameters(previous);
            }

            return await _frameFacade.GoForwardAsync(
                  parameters: parameters);
        }

        // go back

        public event EventHandler CanGoBackChanged;

        public bool CanGoBack()
            => _frameFacade.CanGoBack();

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
            if (parameters == null && _frameFacade is IFrameProvider frameProvider && frameProvider.Frame.BackStack.Any())
            {
                var previous = frameProvider.Frame.BackStack.Last().Parameter?.ToString();
                if (previous is null)
                {
                    parameters = new NavigationParameters();
                }
                else
                {
                    parameters = new NavigationParameters(previous);
                }
            }

            return await _frameFacade.GoBackAsync(
                    parameters: parameters,
                    infoOverride: infoOverride);
        }

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
                return await _frameFacade.NavigateAsync(
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
