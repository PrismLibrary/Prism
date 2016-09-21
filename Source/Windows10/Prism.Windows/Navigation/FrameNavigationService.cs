using Prism.Windows.AppModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Prism.Windows.Navigation
{
    /// <summary>
    /// The <see cref="FrameNavigationService"/> interface is used for navigating across the pages of your Windows Store app.
    /// The <see cref="FrameNavigationService"/> class, uses a class that implements the <see cref="IFrameFacade"/> interface to provide page navigation.
    /// </summary>
    public class FrameNavigationService : INavigationService
    {
        private const string LastNavigationParameterKey = "LastNavigationParameter";
        private const string LastNavigationPageKey = "LastNavigationPageKey";
        private readonly IFrameFacade _frame;
        private readonly Func<string, Type> _navigationResolver;
        private readonly ISessionStateService _sessionStateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameNavigationService"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="navigationResolver">The navigation resolver.</param>
        /// <param name="sessionStateService">The session state service.</param>
        public FrameNavigationService(IFrameFacade frame, Func<string, Type> navigationResolver, ISessionStateService sessionStateService)
        {
            _frame = frame;
            _navigationResolver = navigationResolver;
            _sessionStateService = sessionStateService;

            if (frame != null)
            {
                _frame.NavigatingFrom += OnFrameNavigatingFrom;
                _frame.NavigatedTo += OnFrameNavigatedTo;
            }
        }

        /// <summary>
        /// Navigates to the page with the specified page token, passing the specified parameter.
        /// </summary>
        /// <param name="pageToken">The page token.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Returns <c>true</c> if the navigation succeeds: otherwise, <c>false</c>.</returns>
        public bool Navigate(string pageToken, object parameter)
        {
            Type pageType = _navigationResolver(pageToken);

            if (pageType == null)
            {
                var resourceLoader = ResourceLoader.GetForCurrentView(Constants.InfrastructureResourceMapId);
                var error = string.Format(CultureInfo.CurrentCulture, resourceLoader.GetString("FrameNavigationServiceUnableResolveMessage"), pageToken);
                throw new ArgumentException(error, nameof(pageToken));
            }

            // Get the page type and parameter of the last navigation to check if we
            // are trying to navigate to the exact same page that we are currently on
            var lastNavigationParameter = _sessionStateService.SessionState.ContainsKey(LastNavigationParameterKey) ? _sessionStateService.SessionState[LastNavigationParameterKey] : null;
            var lastPageTypeFullName = _sessionStateService.SessionState.ContainsKey(LastNavigationPageKey) ? _sessionStateService.SessionState[LastNavigationPageKey] as string : string.Empty;

            if (lastPageTypeFullName != pageType.FullName || !AreEquals(lastNavigationParameter, parameter))
            {
                return _frame.Navigate(pageType, parameter);
            }

            return false;
        }


        /// <summary>
        /// Goes to the previous page in the navigation stack.
        /// </summary>
        public void GoBack()
        {
            _frame.GoBack();
        }

        /// <summary>
        /// Determines whether the navigation service can navigate to the previous page or not.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the navigation service can go back; otherwise, <c>false</c>.
        /// </returns>
        public bool CanGoBack()
        {
            return _frame.CanGoBack;
        }

        /// <summary>
        /// Goes to the next page in the navigation stack.
        /// </summary>
        public void GoForward()
        {
            _frame.GoForward();
        }

        /// <summary>
        /// Determines whether the navigation service can navigate to the next page or not.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the navigation service can go forward; otherwise, <c>false</c>.
        /// </returns>
        public bool CanGoForward()
        {
            return _frame.CanGoForward();
        }

        /// <summary>
        /// Clears the navigation history.
        /// </summary>
        public void ClearHistory()
        {
            _frame.ClearBackStack();
        }
        
        /// <summary>
        /// Remove the first page of the backstack with optional pageToken and parameter
        /// </summary>
        /// <param name="pageToken"></param>
        /// <param name="parameter"></param>
        public void RemoveFirstPage(string pageToken = null, object parameter = null)
        {
            PageStackEntry page;
            if (pageToken != null)
            {
                var pageType = _navigationResolver(pageToken);
                if (parameter != null)
                {
                    page = _frame.BackStack.FirstOrDefault(x => x.SourcePageType == pageType && x.Parameter.Equals(parameter));
                }
                else
                {
                    page = _frame.BackStack.FirstOrDefault(x => x.SourcePageType == pageType);
                }
            }
            else
            {
                page = _frame.BackStack.FirstOrDefault();
            }

            if (page != null)
            {
                _frame.RemoveBackStackEntry(page);
            }
        }

        /// <summary>
        /// Remove the last page of the backstack with optional pageToken and parameter
        /// </summary>
        /// <param name="pageToken"></param>
        /// <param name="parameter"></param>
        public void RemoveLastPage(string pageToken = null, object parameter = null)
        {
            PageStackEntry page;
            if (pageToken != null)
            {
                var pageType = _navigationResolver(pageToken);
                if (parameter != null)
                {
                    page = _frame.BackStack.LastOrDefault(x => x.SourcePageType == pageType && x.Parameter.Equals(parameter));
                }
                else
                {
                    page = _frame.BackStack.LastOrDefault(x => x.SourcePageType == pageType);
                }
            }
            else
            {
                page = _frame.BackStack.LastOrDefault();
            }

            if (page != null)
            {
                _frame.RemoveBackStackEntry(page);
            }
        }

        /// <summary>
        /// Remove the all pages of the backstack with optional pageToken and parameter
        /// </summary>
        /// <param name="pageToken"></param>
        /// <param name="parameter"></param>
        public void RemoveAllPages(string pageToken = null, object parameter = null)
        {
            if (pageToken != null)
            {
                IEnumerable<PageStackEntry> pages;
                var pageType = _navigationResolver(pageToken);
                if (parameter != null)
                {
                    pages = _frame.BackStack.Where(x => x.SourcePageType == pageType && x.Parameter.Equals(parameter));
                }
                else
                {
                    pages = _frame.BackStack.Where(x => x.SourcePageType == pageType);
                }

                foreach (var page in pages)
                {
                    _frame.RemoveBackStackEntry(page);
                }
            }
            else
            {
                _frame.ClearBackStack();
            }
        }

        /// <summary>
        /// Restores the saved navigation.
        /// </summary>
        public void RestoreSavedNavigation()
        {
            NavigateToCurrentViewModel(new NavigatedToEventArgs()
            {
                NavigationMode = NavigationMode.Refresh,
                Parameter = _sessionStateService.SessionState[LastNavigationParameterKey]
            });
        }

        /// <summary>
        /// Used for navigating away from the current view model due to a suspension event, in this way you can execute additional logic to handle suspensions.
        /// </summary>
        public void Suspending()
        {
            NavigateFromCurrentViewModel(new NavigatingFromEventArgs(), true);
        }

        /// <summary>
        /// This method is triggered after navigating to a view model. It is used to load the view model state that was saved previously.
        /// </summary>
        /// <param name="e">The <see cref="NavigatedToEventArgs"/> instance containing the event data.</param>
        private void NavigateToCurrentViewModel(NavigatedToEventArgs e)
        {
            var frameState = _sessionStateService.GetSessionStateForFrame(_frame);
            var viewModelKey = "ViewModel-" + _frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                // Clear existing state for forward navigation when adding a new page/view model to the
                // navigation stack
                var nextViewModelKey = viewModelKey;
                int nextViewModelIndex = _frame.BackStackDepth;
                while (frameState.Remove(nextViewModelKey))
                {
                    nextViewModelIndex++;
                    nextViewModelKey = "ViewModel-" + nextViewModelIndex;
                }
            }

            var newView = _frame.Content as FrameworkElement;
            if (newView == null) return;
            var newViewModel = newView.DataContext as INavigationAware;
            if (newViewModel != null)
            {
                Dictionary<string, object> viewModelState;
                if (frameState.ContainsKey(viewModelKey))
                {
                    viewModelState = frameState[viewModelKey] as Dictionary<string, object>;
                }
                else
                {
                    viewModelState = new Dictionary<string, object>();
                }
                newViewModel.OnNavigatedTo(e, viewModelState);
                frameState[viewModelKey] = viewModelState;
            }
        }

        /// <summary>
        /// Navigates away from the current viewmodel.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingFromEventArgs"/> instance containing the event data.</param>
        /// <param name="suspending">True if it is navigating away from the viewmodel due to a suspend event.</param>
        private void NavigateFromCurrentViewModel(NavigatingFromEventArgs e, bool suspending)
        {
            var departingView = _frame.Content as FrameworkElement;
            if (departingView == null) return;
            var frameState = _sessionStateService.GetSessionStateForFrame(_frame);
            var departingViewModel = departingView.DataContext as INavigationAware;

            var viewModelKey = "ViewModel-" + _frame.BackStackDepth;
            if (departingViewModel != null)
            {
                var viewModelState = frameState.ContainsKey(viewModelKey)
                                         ? frameState[viewModelKey] as Dictionary<string, object>
                                         : null;

                departingViewModel.OnNavigatingFrom(e, viewModelState, suspending);
            }
        }

        /// <summary>
        /// Handles the Navigating event of the Frame control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NavigatingFromEventArgs"/> instance containing the event data.</param>
        private void OnFrameNavigatingFrom(object sender, NavigatingFromEventArgs e)
        {
            NavigateFromCurrentViewModel(e, false);
        }

        /// <summary>
        /// Handles the Navigated event of the Frame control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NavigatedToEventArgs"/> instance containing the event data.</param>
        private void OnFrameNavigatedTo(object sender, NavigatedToEventArgs e)
        {
            // Update the page type and parameter of the last navigation
            _sessionStateService.SessionState[LastNavigationPageKey] = _frame.Content.GetType().FullName;
            _sessionStateService.SessionState[LastNavigationParameterKey] = e.Parameter;

            NavigateToCurrentViewModel(e);
        }

        /// <summary>
        /// Returns <c>true</c> if both objects are equal. Two objects are equal if they are null or the same string object.
        /// </summary>
        /// <param name="obj1">First object to compare.</param>
        /// <param name="obj2">Second object to compare.</param>
        /// <returns>Returns <c>true</c> if both parameters are equal; otherwise, <c>false</c>.</returns>
        private static bool AreEquals(object obj1, object obj2)
        {
            if (obj1 != null)
            {
                return obj1.Equals(obj2);
            }
            return obj2 == null;
        }
    }
}
