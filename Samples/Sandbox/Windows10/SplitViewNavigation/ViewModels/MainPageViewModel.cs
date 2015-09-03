using System.Collections.Generic;
using System.Diagnostics;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using SplitViewNavigation.Events;
using Windows.UI.Xaml.Controls;

namespace SplitViewNavigation.ViewModels
{
    public class MainPageViewModel : ViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private bool _isSpacerVisible;

        public bool IsSpacerVisible
        {
            get { return _isSpacerVisible; }
            set { SetProperty(ref _isSpacerVisible, value); }
        }

        /// <summary>
        /// ViewModel for MainPage.
        /// </summary>
        /// <param name="eventAggregator"></param>
        public MainPageViewModel(IEventAggregator eventAggregator)
        {
            Debug.WriteLine("MainPageViewModel()");
            _eventAggregator = eventAggregator;

            //Basic example of how we can use NavigationDisplayModeChangedEvent
            //to adjust things based on the navigation display mode.
            //A page header control that can be included on every page can also
            //be made, and the same technique can be used on that control.
            _eventAggregator.GetEvent<PubSubNames.NavigationDisplayModeChangedEvent>().Subscribe(OnNavigationDisplayModeChangedEvent, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayMode"></param>
        private void OnNavigationDisplayModeChangedEvent(SplitViewDisplayMode displayMode)
        {
            Debug.WriteLine(string.Format("MainPageViewModel.OnNavigationDisplayModeChangedEvent() - DisplayMode: {0}", displayMode));

            IsSpacerVisible = displayMode == SplitViewDisplayMode.Overlay;
        }

        #region INavigationAware Members

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            Debug.WriteLine("MainPageViewModel.OnNavigatingFrom()");
            _eventAggregator.GetEvent<PubSubNames.NavigationDisplayModeChangedEvent>().Unsubscribe(OnNavigationDisplayModeChangedEvent);
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            Debug.WriteLine("MainPageViewModel.OnNavigatedTo()");
            _eventAggregator.GetEvent<PubSubNames.NavigatedToPageEvent>().Publish(e);
            base.OnNavigatedTo(e, viewModelState);
        }

        #endregion
    }
}
