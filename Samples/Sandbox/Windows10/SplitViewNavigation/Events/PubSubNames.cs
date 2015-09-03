using Prism.Events;
using Prism.Windows.Navigation;
using SplitViewNavigation.Models;
using Windows.UI.Xaml.Controls;

namespace SplitViewNavigation.Events
{
    /// <summary>
    /// A wrapper class for PubSubEvent classes for easy access.
    /// </summary>
    public class PubSubNames
    {
        #region Navigation

        /// <summary>
        /// PubSubEvent that occurs when a page has been navigated to.
        /// </summary>
        public class NavigatedToPageEvent : PubSubEvent<NavigatedToEventArgs> { }

        /// <summary>
        /// PubSubEvent that occurs when a navigation item has been invoked.
        /// </summary>
        public class NavigationItemInvokedEvent : PubSubEvent<NavigationItem> { }

        /// <summary>
        /// PubSubEvent that occurs when the navigation display mode has changed.
        /// </summary>
        public class NavigationDisplayModeChangedEvent : PubSubEvent<SplitViewDisplayMode> { }

        /// <summary>
        /// PubSubEvent that occurs when the navigation pane has been opened.
        /// </summary>
        public class NavigationPaneOpenedEvent : PubSubEvent<object> { }

        /// <summary>
        /// PubSubEvent that occurs when the navigation pane has been closed.
        /// </summary>
        public class NavigationPaneClosedEvent : PubSubEvent<object> { }

        #endregion
    }
}
