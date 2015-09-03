using System.Collections.Generic;
using System.Diagnostics;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using SplitViewNavigation.Events;

namespace SplitViewNavigation.ViewModels
{
    class Page5PageViewModel : ViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventAggregator"></param>
        public Page5PageViewModel(IEventAggregator eventAggregator)
        {
            Debug.WriteLine("Page5PageViewModel()");
            _eventAggregator = eventAggregator;
        }

        #region INavigationAware Members

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            Debug.WriteLine("Page5PageViewModel.OnNavigatingFrom()");
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            Debug.WriteLine("Page5PageViewModel.OnNavigatedTo()");
            _eventAggregator.GetEvent<PubSubNames.NavigatedToPageEvent>().Publish(e);
            base.OnNavigatedTo(e, viewModelState);
        }

        #endregion
    }
}
