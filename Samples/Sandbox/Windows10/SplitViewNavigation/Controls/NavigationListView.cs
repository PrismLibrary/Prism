using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Unity.Windows;
using Prism.Windows.Navigation;
using SplitViewNavigation.Events;
using SplitViewNavigation.Models;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SplitViewNavigation.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class NavigationListView : ListView
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;
        private SplitView _splitViewHost;
        private int _selectedIndex;

        /// <summary>
        /// 
        /// </summary>
        public NavigationListView()
        {
            Debug.WriteLine("NavigationListView()");

            //Designer doesn't like PrismUnityApplication.Current and sometimes throws a NullReferenceException
            if (!DesignMode.DesignModeEnabled)
            {
                _eventAggregator = PrismUnityApplication.Current.Container.Resolve<IEventAggregator>();
                _navigationService = PrismUnityApplication.Current.Container.Resolve<INavigationService>();

                _eventAggregator.GetEvent<PubSubNames.NavigatedToPageEvent>().Subscribe(OnNavigatedToPageEvent, true);
            }

            _selectedIndex = -1;

            IsItemClickEnabled = true;
            Loaded += OnLoaded;
            ItemClick += OnItemClick;
            SelectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("NavigationListView.OnLoaded()");

            DependencyObject parent = VisualTreeHelper.GetParent(this);

            //Look for the parent SplitView
            while (parent != null && !(parent is SplitView))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                _splitViewHost = (SplitView)parent;

                RegisterSplitViewCallbacks();

                Frame _rootFrame = _splitViewHost.Content as Frame;

                //Select navigation item based on the initial page type if present.
                if (_rootFrame != null && _rootFrame.Content != null)
                {
                    Debug.WriteLine(string.Format("NavigationListView.OnLoaded() - Found rootFrame.Content: {0}", _rootFrame.Content.GetType().Name));
                    SetSelectedItem(_rootFrame.Content.GetType());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //When clicking an item the selection is updated immediately.
            //But if the navigation is canceled we have to set the selected index back to the previous selected index.
            if (_selectedIndex != SelectedIndex)
            {
                Debug.WriteLine(string.Format("NavigationListView.OnSelectionChanged() - Resetting SelectedIndex from {0} to {1}",
                    SelectedIndex, _selectedIndex));
                SelectedIndex = _selectedIndex;
            }
            else
            {
                _selectedIndex = SelectedIndex;
                Debug.WriteLine(string.Format("NavigationListView.OnSelectionChanged() - SelectedIndex: {0}", SelectedIndex));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Debug.WriteLine(string.Format("NavigationListView.OnItemClick() - SelectedIndex: {0}", SelectedIndex));

                NavigationItem item = (NavigationItem)e.ClickedItem;

                _eventAggregator.GetEvent<PubSubNames.NavigationItemInvokedEvent>().Publish(item);

                _navigationService.Navigate(item.Key, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("NavigationListView.OnItemClick() - Error: {0}", ex.Message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnNavigatedToPageEvent(NavigatedToEventArgs e)
        {
            Debug.WriteLine(string.Format("NavigationListView.OnNavigatedToPageEvent() - pageType: {0}",
                e.SourcePageType.Name));

            SetSelectedItem(e.SourcePageType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePageType"></param>
        private void SetSelectedItem(Type sourcePageType)
        {
            Debug.WriteLine(string.Format("NavigationListView.SetSelectedItem(Type sourcePageType) - pageType: {0}",
                sourcePageType.Name));

            var item = Items.OfType<NavigationItem>()
                .Where(navItem => sourcePageType.Name.StartsWith(navItem.Key, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            //if (item == null && _navigationService.BackStackDepth > 0)
            //{
            //    // In cases where a page drills into sub-pages then we'll highlight the most recent
            //    // navigation menu item that appears in the BackStack
            //    foreach (var entry in _navigationService.BackStack.Reverse())
            //    {
            //        item = Items.OfType<NavigationItem>()
            //            .Where(navItem => entry.SourcePageType.Name.StartsWith(navItem.Key, StringComparison.OrdinalIgnoreCase))
            //            .FirstOrDefault();

            //        if (item != null)
            //            break;
            //    }
            //}

            if (item != null)
                SetSelectedItem(item);
            else
                Debug.WriteLine(string.Format("NavigationListView.SetSelectedItem(Type sourcePageType) - pageType: {0} not found",
                    sourcePageType.Name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void SetSelectedItem(NavigationItem item)
        {
            Debug.WriteLine(string.Format("NavigationListView.SetSelectedItem(NavigationItem item) - item.Key: {0}",
                item.Key));

            ListViewItem container = (ListViewItem)ContainerFromItem(item);

            if (container == null)
                return;

            //// While updating the selection state of the item prevent it from taking keyboard focus.  If a
            //// user is invoking the back button via the keyboard causing the selected nav menu item to change
            //// then focus will remain on the back button.
            if (container != null) container.IsTabStop = false;

            int index = IndexFromContainer(container);

            for (int i = 0; i < Items.Count; i++)
            {
                var lvi = (ListViewItem)ContainerFromIndex(i);

                if (lvi == null) continue;

                if (i == index)
                {
                    Debug.WriteLine(string.Format("NavigationListView.SetSelectedItem(NavigationItem item) - Selecting index: {0}", i));
                    _selectedIndex = i;
                    lvi.IsSelected = true;
                }
                else
                {
                    //Debug.WriteLine(string.Format("NavigationListView.SetSelectedItem(NavigationItem item) - UnSelecting index: {0}", i));
                    //lvi.IsSelected = false;
                }
            }

            if (container != null) container.IsTabStop = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RegisterSplitViewCallbacks()
        {
            _splitViewHost.RegisterPropertyChangedCallback(SplitView.IsPaneOpenProperty, (sender, args) =>
            {
                OnPaneToggled();
            });

            // Call once to ensure we're in the correct state
            OnPaneToggled();
        }

        /// <summary>
        /// Re-size the ListView's Panel when the SplitView is compact so the items
        /// will fit within the visible space and correctly display a keyboard focus rect.
        /// </summary>
        private void OnPaneToggled()
        {
            if (_splitViewHost.IsPaneOpen)
            {
                ItemsPanelRoot.ClearValue(FrameworkElement.WidthProperty);
                ItemsPanelRoot.ClearValue(FrameworkElement.HorizontalAlignmentProperty);
            }
            else if (_splitViewHost.DisplayMode == SplitViewDisplayMode.CompactInline ||
                  _splitViewHost.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                ItemsPanelRoot.SetValue(FrameworkElement.WidthProperty, _splitViewHost.CompactPaneLength);
                ItemsPanelRoot.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Remove the entrance animation on the item containers.
            for (int i = 0; i < ItemContainerTransitions.Count; i++)
            {
                if (ItemContainerTransitions[i] is EntranceThemeTransition)
                {
                    ItemContainerTransitions.RemoveAt(i);
                }
            }
        }
    }
}
