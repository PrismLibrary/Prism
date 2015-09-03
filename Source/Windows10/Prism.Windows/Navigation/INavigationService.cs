namespace Prism.Windows.Navigation
{
    /// <summary>
    /// The INavigationService interface is used for creating a navigation service for your Windows Store app.
    /// The default implementation of INavigationService is the FrameNavigationService class, that uses a class that implements the IFrameFacade interface
    /// to provide page navigation.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to the page with the specified page token, passing the specified parameter.
        /// </summary>
        /// <param name="pageToken">The page token.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Returns <c>true</c> if navigation succeeds; otherwise, <c>false</c></returns>
        bool Navigate(string pageToken, object parameter);

        /// <summary>
        /// Goes to the previous page in the navigation stack.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Determines whether the navigation service can navigate to the previous page or not.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the navigation service can go back; otherwise, <c>false</c>.
        /// </returns>
        bool CanGoBack();

        /// <summary>
        /// Goes to the next page in the navigation stack.
        /// </summary>
        void GoForward();

        /// <summary>
        /// Determines whether the navigation service can navigate to the next page or not.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the navigation service can go forward; otherwise, <c>false</c>.
        /// </returns>
        bool CanGoForward();

        /// <summary>
        /// Clears the navigation history.
        /// </summary>
        void ClearHistory();

        /// <summary>
        /// Remove the first page of the backstack with optional pageToken and parameter
        /// </summary>
        /// <param name="pageToken"></param>
        /// <param name="parameter"></param>
        void RemoveFirstPage(string pageToken = null, object parameter = null);

        /// <summary>
        /// Remove the last page of the backstack with optional pageToken and parameter
        /// </summary>
        /// <param name="pageToken"></param>
        /// <param name="parameter"></param>
        void RemoveLastPage(string pageToken = null, object parameter = null);

        /// <summary>
        /// Remove the all pages of the backstack with optional pageToken and parameter
        /// </summary>
        /// <param name="pageToken"></param>
        /// <param name="parameter"></param>
        void RemoveAllPages(string pageToken = null, object parameter = null);
        /// <summary>
        /// Restores the saved navigation.
        /// </summary>
        void RestoreSavedNavigation();

        /// <summary>
        /// Used for navigating away from the current view model due to a suspension event, in this way you can execute additional logic to handle suspensions.
        /// </summary>
        void Suspending();
    }
}
