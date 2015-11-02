using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Prism.Windows.Navigation
{
    /// <summary>
    /// The <see cref="IFrameFacade"/> interface abstracts the <see cref="Frame"/> object for use by apps that derive
    /// from the <see cref="PrismApplication"/> class. A <see cref="Frame"/> represents a content control that supports navigation.
    /// The default implementation of <see cref="IFrameFacade"/> is the <see cref="FrameFacadeAdapter"/> class, 
    /// which simply passes method invocations to an underlying <see cref="Frame"/> object. 
    /// 
    /// However, in addition to the <see cref="FrameFacadeAdapter"/>, test environments may implement 
    /// this interface for the purposes of unit testing and integration testing. 
    /// </summary>
    public interface IFrameFacade
    {
        /// <summary>
        /// Gets or sets the content of a ContentControl.
        /// </summary>
        /// 
        /// <returns>
        /// An object that contains the control's content. The default is null.
        /// </returns>
        object Content { get; set; }

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a <see cref="Frame"/> manages its own navigation history.
        /// </summary>
        void GoBack();

        /// <returns>
        /// The string-form serialized navigation history. See Remarks.
        /// </returns>
        string GetNavigationState();

        /// <summary>
        /// Reads and restores the navigation history of a <see cref="Frame"/> from a provided serialization string.
        /// </summary>
        /// <param name="navigationState">The serialization string that supplies the restore point for navigation history.</param>
        void SetNavigationState(string navigationState);

        /// <summary>
        /// Navigates to a page of the requested type.
        /// </summary>
        /// <param name="sourcePageType">The type of the page that will be navigated to.</param>
        /// <param name="parameter">The page's navigation parameter.</param>
        /// 
        /// <returns>True if navigation was successful; false otherwise.</returns>
        bool Navigate(Type sourcePageType, object parameter);

        /// <summary>
        /// Gets the number of entries in the navigation back stack.
        /// </summary>
        /// 
        /// <returns>
        /// The number of entries in the navigation back stack.
        /// </returns>
        int BackStackDepth { get; }

        IReadOnlyList<PageStackEntry> BackStack { get; }

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
        /// Gets a value that indicates whether there is at least one entry in back navigation history.
        /// </summary>
        /// 
        /// <returns>
        /// True if there is at least one entry in back navigation history; false if there are no entries in back navigation history or the Frame does not own its own navigation history.
        /// </returns>
        bool CanGoBack { get; }

        /// <summary>
        /// Remove a <see cref="PageStackEntry"/> from the <see cref="Frame"/>'s back stack.
        /// </summary>
        /// <param name="entry">The <see cref="PageStackEntry"/> to remove.</param>
        /// <returns>True if item was successfully removed from the back stack; otherwise, false. This method also returns false if item is not found in the back stack.</returns>
        bool RemoveBackStackEntry(PageStackEntry entry);

        /// <summary>
        /// Clears the Frame's back stack.
        /// </summary>
        void ClearBackStack();

        /// <summary>
        /// Occurs when the content that is being navigated to has been found and is available from the Content property, although it may not have completed loading.
        /// </summary>
        event EventHandler<NavigatedToEventArgs> NavigatedTo;

        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        event EventHandler<NavigatingFromEventArgs> NavigatingFrom;

        /// <summary>
        /// Returns the current effective value of a dependency property from a <see cref="DependencyObject"/>.
        /// </summary>
        /// 
        /// <returns>
        /// Returns the current effective value.
        /// </returns>
        /// <param name="dependencyProperty">The <see cref="DependencyProperty"/> identifier of the property for which to retrieve the value.</param>
        object GetValue(DependencyProperty dependencyProperty);

        /// <summary>
        /// Sets the local value of a dependency property on a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="dependencyProperty">The identifier of the dependency property to set.</param>
        /// <param name="value">The new local value.</param>
        void SetValue(DependencyProperty dependencyProperty, object value);

        /// <summary>
        /// Clears the local value of a dependency property.
        /// </summary>
        /// <param name="dependencyProperty">The <see cref="DependencyProperty"/> identifier of the property for which to clear the value.</param>
        void ClearValue(DependencyProperty dependencyProperty);
    }
}
