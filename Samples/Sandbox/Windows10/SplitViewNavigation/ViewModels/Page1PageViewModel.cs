using System.Collections.Generic;
using System.Diagnostics;
using Prism.Events;
using Prism.Windows.Mvvm;
using SplitViewNavigation.Events;

namespace SplitViewNavigation.ViewModels
{
    class Page1PageViewModel : ViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventAggregator"></param>
        public Page1PageViewModel(IEventAggregator eventAggregator)
        {
            Debug.WriteLine("Page1PageViewModel()");
            _eventAggregator = eventAggregator;
        }

        #region INavigationAware Members

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            Debug.WriteLine("Page1PageViewModel.OnNavigatingFrom()");
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            Debug.WriteLine("Page1PageViewModel.OnNavigatedTo()");
            _eventAggregator.GetEvent<PubSubNames.NavigatedToPageEvent>().Publish(e);
            base.OnNavigatedTo(e, viewModelState);
        }

        #endregion
    }
}
