using System.Diagnostics;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Unity.Windows;
using SplitViewNavigation.Events;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

namespace SplitViewNavigation.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class NavigationSplitView : SplitView
    {
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// 
        /// </summary>
        public NavigationSplitView()
        {
            Debug.WriteLine("NavigationSplitView()");

            //Designer doesn't like PrismUnityApplication.Current and sometimes throws a NullReferenceException
            if (!DesignMode.DesignModeEnabled)
            {
                _eventAggregator = PrismUnityApplication.Current.Container.Resolve<IEventAggregator>();
            }

            RegisterSplitViewCallbacks();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RegisterSplitViewCallbacks()
        {
            RegisterPropertyChangedCallback(DisplayModeProperty, (sender, args) =>
            {
                _eventAggregator.GetEvent<PubSubNames.NavigationDisplayModeChangedEvent>().Publish(DisplayMode);
            });

            RegisterPropertyChangedCallback(IsPaneOpenProperty, (sender, args) =>
            {
                if (IsPaneOpen)
                    _eventAggregator.GetEvent<PubSubNames.NavigationPaneOpenedEvent>().Publish(string.Empty);
                else
                    _eventAggregator.GetEvent<PubSubNames.NavigationPaneClosedEvent>().Publish(string.Empty);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
