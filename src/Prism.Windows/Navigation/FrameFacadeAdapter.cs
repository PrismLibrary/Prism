using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Prism.Windows.Navigation
{
    /// <summary>
    /// Abstracts the <see cref="Frame"/> object for use by apps that derive from the <see cref="PrismApplication"/> class.
    /// </summary>
    public class FrameFacadeAdapter : IFrameFacade
    {
        private readonly Frame _frame;
        private readonly NavigationStateChangedEvent _navigationStateChangedEvent;
        private readonly List<EventHandler<NavigatedToEventArgs>> _navigatedToEventHandlers = new List<EventHandler<NavigatedToEventArgs>>();
        private readonly List<EventHandler<NavigatingFromEventArgs>> _navigatingFromEventHandlers = new List<EventHandler<NavigatingFromEventArgs>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameFacadeAdapter"/> class.
        /// </summary>
        /// <param name="frame">The <see cref="Frame"/> that will be wrapped.</param>
        /// <param name="eventAggregator">The event aggregator to be used for publishing a <see cref="NavigationStateChangedEvent"/>. Can be null if events aren't needed.</param>
        public FrameFacadeAdapter(Frame frame, IEventAggregator eventAggregator = null)
        {
            if (eventAggregator != null)
            {
                _navigationStateChangedEvent = eventAggregator.GetEvent<NavigationStateChangedEvent>();
            }

            _frame = frame;
        }

        /// <summary>
        /// Gets or sets the content of a ContentControl.
        /// </summary>
        ///
        /// <returns>
        /// An object that contains the control's content. The default is null.
        /// </returns>
        public object Content
        {
            get { return _frame.Content; }
            set { _frame.Content = value; }
        }

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a Frame manages its own navigation history.
        /// </summary>
        public void GoBack()
        {
            _frame.GoBack();
            _navigationStateChangedEvent?.Publish(new NavigationStateChangedEventArgs(this, StateChangeType.Back));
        }

        /// <returns>
        /// The string-form serialized navigation history. See Remarks.
        /// </returns>
        public string GetNavigationState()
        {
            var navigationState = _frame.GetNavigationState();
            return navigationState;
        }

        /// <summary>
        /// Reads and restores the navigation history of a Frame from a provided serialization string.
        /// </summary>
        /// <param name="navigationState">The serialization string that supplies the restore point for navigation history.</param>
        public void SetNavigationState(string navigationState)
        {
            _frame.SetNavigationState(navigationState);
            _navigationStateChangedEvent?.Publish(new NavigationStateChangedEventArgs(this, StateChangeType.Set));
        }

        /// <summary>
        /// Navigates to a page of the requested type.
        /// </summary>
        /// <param name="sourcePageType">The type of the page that will be navigated to.</param>
        /// <param name="parameter">The page's navigation parameter.</param>
        ///
        /// <returns>True if navigation was successful; false otherwise.</returns>
        public bool Navigate(Type sourcePageType, object parameter)
        {
            try
            {
                var success = _frame.Navigate(sourcePageType, parameter);

                if (success)
                {
                    _navigationStateChangedEvent?.Publish(new NavigationStateChangedEventArgs(this, StateChangeType.Forward));
                }

                return success;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of entries in the navigation back stack.
        /// </summary>
        ///
        /// <returns>
        /// The number of entries in the navigation back stack.
        /// </returns>
        public int BackStackDepth
        {
            get { return _frame.BackStackDepth; }
        }

        public IReadOnlyList<PageStackEntry> BackStack => _frame.BackStack.ToList();

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation history.
        /// </summary>
        ///
        /// <returns>
        /// True if there is at least one entry in back navigation history; false if there are no entries in back navigation history or the Frame does not own its own navigation history.
        /// </returns>
        public bool CanGoBack
        {
            get { return _frame.CanGoBack; }
        }

        /// <summary>
        /// Goes to the next page in the navigation stack.
        /// </summary>
        public void GoForward()
        {
            _frame.GoForward();
            _navigationStateChangedEvent?.Publish(new NavigationStateChangedEventArgs(this, StateChangeType.Forward));
        }

        /// <summary>
        /// Determines whether the navigation service can navigate to the next page or not.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the navigation service can go forward; otherwise, <c>false</c>.
        /// </returns>
        public bool CanGoForward()
        {
            return _frame.CanGoForward;
        }

        /// <summary>
        /// Remove a <see cref="PageStackEntry"/> from the Frame's back stack.
        /// </summary>
        /// <param name="entry">The <see cref="PageStackEntry"/> to remove.</param>
        /// <returns>True if item was successfully removed from the back stack; otherwise, false. This method also returns false if item is not found in the back stack.</returns>
        public bool RemoveBackStackEntry(PageStackEntry entry)
        {
            var success = _frame.BackStack.Remove(entry);
            if (success)
            {
                _navigationStateChangedEvent?.Publish(new NavigationStateChangedEventArgs(this, StateChangeType.Remove));
            }
            
            return success;
        }

        /// <summary>
        /// Clears the Frame's back stack.
        /// </summary>
        /// <returns></returns>
        public void ClearBackStack()
        {
            _frame.BackStack.Clear();
            _navigationStateChangedEvent?.Publish(new NavigationStateChangedEventArgs(this, StateChangeType.Clear));
        }

        /// <summary>
        /// Occurs when the content that is being navigated to has been found and is available from the Content property, although it may not have completed loading.
        /// </summary>
        public event EventHandler<NavigatedToEventArgs> NavigatedTo
        {
            add
            {
                if (_navigatedToEventHandlers.Contains(value)) return;
                _navigatedToEventHandlers.Add(value);

                if (_navigatedToEventHandlers.Count == 1)
                {
                    _frame.Navigated += OnFrameNavigatedTo;
                }
            }

            remove
            {
                if (!_navigatedToEventHandlers.Contains(value)) return;
                _navigatedToEventHandlers.Remove(value);

                if (_navigatedToEventHandlers.Count == 0)
                {
                    _frame.Navigated -= OnFrameNavigatedTo;
                }
            }
        }

        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        public event EventHandler<NavigatingFromEventArgs> NavigatingFrom
        {
            add
            {
                if (_navigatingFromEventHandlers.Contains(value)) return;
                _navigatingFromEventHandlers.Add(value);

                if (_navigatingFromEventHandlers.Count == 1)
                {
                    _frame.Navigating += OnFrameNavigatingFrom;
                }
            }

            remove
            {
                if (!_navigatingFromEventHandlers.Contains(value)) return;
                _navigatingFromEventHandlers.Remove(value);

                if (_navigatingFromEventHandlers.Count == 0)
                {
                    _frame.Navigating -= OnFrameNavigatingFrom;
                }
            }
        }

        /// <summary>
        /// Returns the current effective value of a dependency property from a <see cref="DependencyObject"/>.
        /// </summary>
        ///
        /// <returns>
        /// Returns the current effective value.
        /// </returns>
        /// <param name="dependencyProperty">The <see cref="DependencyProperty"/> identifier of the property for which to retrieve the value.</param>
        public object GetValue(DependencyProperty dependencyProperty)
        {
            return _frame.GetValue(dependencyProperty);
        }

        /// <summary>
        /// Sets the local value of a dependency property on a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="dependencyProperty">The identifier of the dependency property to set.</param><param name="value">The new local value.</param>
        public void SetValue(DependencyProperty dependencyProperty, object value)
        {
            _frame.SetValue(dependencyProperty, value);
        }

        /// <summary>
        /// Clears the local value of a dependency property.
        /// </summary>
        /// <param name="dependencyProperty">The <see cref="DependencyProperty"/> identifier of the property for which to clear the value.</param>
        public void ClearValue(DependencyProperty dependencyProperty)
        {
            _frame.ClearValue(dependencyProperty);
        }

        private void OnFrameNavigatedTo(object sender, NavigationEventArgs e)
        {
            if (_navigatedToEventHandlers.Count > 0)
            {
                var args = new NavigatedToEventArgs(e);

                foreach (var handler in _navigatedToEventHandlers)
                    handler(this, args);
            }
        }

        private void OnFrameNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (_navigatingFromEventHandlers.Count > 0)
            {
                var args = new NavigatingFromEventArgs(e);

                foreach (var handler in _navigatingFromEventHandlers)
                    handler(this, args);
            }
        }
    }
}
