using System.Collections.Generic;
using System.Diagnostics;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using SplitViewNavigation.Events;

namespace SplitViewNavigation.ViewModels
{
    class Page4PageViewModel : ViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private bool _isNavigationDisabled;

        public bool IsNavigationDisabled
        {
            get { return _isNavigationDisabled; }
            set { SetProperty(ref _isNavigationDisabled, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventAggregator"></param>
        public Page4PageViewModel(IEventAggregator eventAggregator)
        {
            Debug.WriteLine("Page4PageViewModel()");
            _eventAggregator = eventAggregator;

            _isNavigationDisabled = true;
        }

        #region INavigationAware Members

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            Debug.WriteLine("Page4PageViewModel.OnNavigatingFrom()");
            e.Cancel = _isNavigationDisabled;
            if (_isNavigationDisabled)
                Debug.WriteLine("Page4PageViewModel.OnNavigatingFrom() - Canceled");
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            Debug.WriteLine("Page4PageViewModel.OnNavigatedTo()");
            _eventAggregator.GetEvent<PubSubNames.NavigatedToPageEvent>().Publish(e);
            base.OnNavigatedTo(e, viewModelState);
        }

        #endregion
    }
}
