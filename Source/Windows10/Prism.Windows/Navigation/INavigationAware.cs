using System.Collections.Generic;

namespace Prism.Windows.Navigation
{
    /// <summary>
    /// The INavigationAware interface should be used for view models that require persisting and loading state due to suspend/resume events.
    /// The Prism.Windows.Mvvm.ViewModel base class implements this interface, therefore every view model that inherits from it
    /// will support the navigation aware methods. 
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Called when navigation is performed to a page. You can use this method to load state if it is available.
        /// </summary>
        /// <param name="e">The <see cref="NavigatedToEventArgs"/> instance containing the event data.</param>
        /// <param name="viewModelState">The state of the view model.</param>
        void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState);

        /// <summary>
        /// This method will be called when navigating away from a page. You can use this method to save your view model data in case of a suspension event.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingFromEventArgs"/> instance containing the event data.</param>
        /// <param name="viewModelState">The state of the view model.</param>
        /// <param name="suspending">If set to <c>true</c> you are navigating away from this view model due to a suspension event.</param>
        void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending);
    }
}
