using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Prism.Commands;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using SplitViewNavigation.Events;
using SplitViewNavigation.Models;
using Windows.UI.Xaml.Controls;

namespace SplitViewNavigation.ViewModels
{
    /// <summary>
    /// ViewModel for ShellPage.
    /// </summary>
    public class ShellPageViewModel : ViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;

        #region Properties

        private ObservableCollection<NavigationItem> _navigationItems;

        public ObservableCollection<NavigationItem> NavigationItems
        {
            get { return _navigationItems; }
            set { SetProperty(ref _navigationItems, value); }
        }

        private bool _isNavigationToggleButtonEnabled;

        public bool IsNavigationToggleButtonEnabled
        {
            get { return _isNavigationToggleButtonEnabled; }
            set { SetProperty(ref _isNavigationToggleButtonEnabled, value); }
        }

        private bool _isNavigationToggleButtonVisible;

        public bool IsNavigationButtonVisible
        {
            get { return _isNavigationToggleButtonVisible; }
            set { SetProperty(ref _isNavigationToggleButtonVisible, value); }
        }

        private bool _isNavigationPaneOpen;

        public bool IsNavigationPaneOpen
        {
            get { return _isNavigationPaneOpen; }
            set { SetProperty(ref _isNavigationPaneOpen, value); }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventAggregator"></param>
        /// <param name="navigationService"></param>
        public ShellPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
        {
            Debug.WriteLine("ShellPageViewModel()");
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;

            IsNavigationButtonVisible = true;
            IsNavigationToggleButtonEnabled = true;

            NavigationToggleCommand = new DelegateCommand(OnNavigationToggleClick);

            NavigationItems = new ObservableCollection<NavigationItem>();

            NavigationItems.Add(new NavigationItem() { Key = "Main", Text = "Home", Glyph = "\uE10F" });
            NavigationItems.Add(new NavigationItem() { Key = "Page1", Text = "Page 1", Glyph = "\uE128" });
            NavigationItems.Add(new NavigationItem() { Key = "Page2", Text = "Page 2", Glyph = "\uE129" });
            NavigationItems.Add(new NavigationItem() { Key = "Page3", Text = "Page 3", Glyph = "\uE130" });
            NavigationItems.Add(new NavigationItem() { Key = "Page4", Text = "Page 4 - Nav. Locking", Glyph = "\uE131" });
            NavigationItems.Add(new NavigationItem() { Key = "Page5", Text = "Page 5", Glyph = "\uE132" });

            _eventAggregator.GetEvent<PubSubNames.NavigationPaneClosedEvent>().Subscribe(OnNavigationPaneClosedEvent, true);
            _eventAggregator.GetEvent<PubSubNames.NavigationPaneOpenedEvent>().Subscribe(OnNavigationPaneOpenedEvent, true);
            _eventAggregator.GetEvent<PubSubNames.NavigationDisplayModeChangedEvent>().Subscribe(OnNavigationDisplayModeChangedEvent, true);
            _eventAggregator.GetEvent<PubSubNames.NavigationItemInvokedEvent>().Subscribe(OnNavigationItemInvokedEvent, true);
        }

        #region PubSubEvents

        private void OnNavigationPaneClosedEvent(object obj)
        {
            Debug.WriteLine("ShellPageViewModel.OnNavigationPaneClosedEvent");
        }

        private void OnNavigationPaneOpenedEvent(object obj)
        {
            Debug.WriteLine("ShellPageViewModel.OnNavigationPaneOpenedEvent");
        }

        private void OnNavigationDisplayModeChangedEvent(SplitViewDisplayMode displayMode)
        {
            Debug.WriteLine(string.Format("ShellPageViewModel.OnNavigationDisplayModeChangedEvent() - DisplayMode: {0}", displayMode));
        }

        private void OnNavigationItemInvokedEvent(NavigationItem item)
        {
            Debug.WriteLine("ShellPageViewModel.OnNavigationItemInvokedEvent");
            if (IsNavigationPaneOpen)
            {
                IsNavigationPaneOpen = false;
            }
        }

        #endregion

        #region Commands

        public DelegateCommand NavigationToggleCommand { get; private set; }

        private void OnNavigationToggleClick()
        {
            IsNavigationPaneOpen = !IsNavigationPaneOpen;
        }

        #endregion

        #region INavigationAware Members

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            Debug.WriteLine("ShellPageViewModel.OnNavigatingFrom()");
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            Debug.WriteLine("ShellPageViewModel.OnNavigatedTo()");
            _eventAggregator.GetEvent<PubSubNames.NavigatedToPageEvent>().Publish(e);
            base.OnNavigatedTo(e, viewModelState);
        }

        #endregion
    }
}
