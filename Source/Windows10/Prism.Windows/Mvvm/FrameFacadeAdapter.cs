using Prism.Windows.Interfaces;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Prism.Windows.Mvvm
{
    /// <summary>
    /// Abstracts the Windows.UI.Xaml.Controls.Frame object for use by apps that derive from the PrismApplication class.
    /// </summary>
    public class FrameFacadeAdapter : IFrameFacade
    {
        private readonly Frame _frame;
        private readonly List<EventHandler<MvvmNavigatedEventArgs>> _navigatedEventHandlers = new List<EventHandler<MvvmNavigatedEventArgs>>();
        private readonly List<EventHandler> _navigatingEventHandlers = new List<EventHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameFacadeAdapter"/> class.
        /// </summary>
        /// <param name="frame">The Frame that will be wrapped.</param>
        public FrameFacadeAdapter(Frame frame)
        {
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
                return _frame.Navigate(sourcePageType, parameter);
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
        /// Occurs when the content that is being navigated to has been found and is available from the Content property, although it may not have completed loading.
        /// </summary>
        public event EventHandler<MvvmNavigatedEventArgs> Navigated
        {
            add
            {
                if (_navigatedEventHandlers.Contains(value)) return;
                _navigatedEventHandlers.Add(value);

                if (_navigatedEventHandlers.Count == 1)
                {
                    _frame.Navigated += FacadeNavigatedEventHandler;
                }
            }

            remove
            {
                if (!_navigatedEventHandlers.Contains(value)) return;
                _navigatedEventHandlers.Remove(value);

                if (_navigatedEventHandlers.Count == 0)
                {
                    _frame.Navigated -= FacadeNavigatedEventHandler;
                }
            }
        }

        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        public event EventHandler Navigating
        {
            add
            {
                if (_navigatingEventHandlers.Contains(value)) return;
                _navigatingEventHandlers.Add(value);

                if (_navigatingEventHandlers.Count == 1)
                {
                    _frame.Navigating += FacadeNavigatingCancelEventHandler;
                }
            }

            remove
            {
                if (!_navigatingEventHandlers.Contains(value)) return;
                _navigatingEventHandlers.Remove(value);

                if (_navigatingEventHandlers.Count == 0)
                {
                    _frame.Navigating -= FacadeNavigatingCancelEventHandler;
                }
            }
        }

        /// <summary>
        /// Returns the current effective value of a dependency property from a DependencyObject.
        /// </summary>
        /// 
        /// <returns>
        /// Returns the current effective value.
        /// </returns>
        /// <param name="dependencyProperty">The DependencyProperty identifier of the property for which to retrieve the value.</param>
        public object GetValue(DependencyProperty dependencyProperty)
        {
            return _frame.GetValue(dependencyProperty);
        }

        /// <summary>
        /// Sets the local value of a dependency property on a DependencyObject.
        /// </summary>
        /// <param name="dependencyProperty">The identifier of the dependency property to set.</param><param name="value">The new local value.</param>
        public void SetValue(DependencyProperty dependencyProperty, object value)
        {
            _frame.SetValue(dependencyProperty, value);
        }

        /// <summary>
        /// Clears the local value of a dependency property.
        /// </summary>
        /// <param name="dependencyProperty">The DependencyProperty identifier of the property for which to clear the value.</param>
        public void ClearValue(DependencyProperty dependencyProperty)
        {
            _frame.ClearValue(dependencyProperty);
        }

        private void FacadeNavigatedEventHandler(object sender, NavigationEventArgs e)
        {
            foreach (var handler in _navigatedEventHandlers)
            {
                var eventArgs = new MvvmNavigatedEventArgs()
                {
                    NavigationMode = e.NavigationMode,
                    Parameter = e.Parameter
                };
                handler(this, eventArgs);
            }
        }

        private void FacadeNavigatingCancelEventHandler(object sender, NavigatingCancelEventArgs e)
        {
            foreach (var handler in _navigatingEventHandlers)
            {
                handler(this, new EventArgs());
            }
        }

    }
}
