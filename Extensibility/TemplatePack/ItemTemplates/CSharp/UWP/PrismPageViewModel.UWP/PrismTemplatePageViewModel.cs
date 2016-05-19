using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $rootnamespace$
{
    public class $safeitemname$ : ViewModelBase
    {
        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            if (!suspending)
            {
                // TODO: Clean up the ViewModel here.
                // Cleanup should not be done if the app is suspending, since OnNavigatedTo is not called on resume.
                // If the app suspends and subsequently terminates, the ViewModel will get cleaned up implicitly.
            }

            base.OnNavigatingFrom(e, viewModelState, suspending);
        }
    }
}
