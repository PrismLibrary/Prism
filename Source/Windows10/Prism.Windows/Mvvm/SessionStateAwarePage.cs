using System;
using System.Collections.Generic;
using Prism.Commands;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Prism.Windows.Navigation;

namespace Prism.Windows.Mvvm
{
    /// <summary>
    /// This is the base class that can be used for pages that need to be aware of layout changes and update the visual state accordingly.
    /// </summary>
    public class SessionStateAwarePage : Page
    {

        /// <summary>
        /// Gets or sets the get session state for Frame.
        /// </summary>
        /// <value>
        /// The session state for the Frame.
        /// </value>
        public static Func<IFrameFacade, IDictionary<string, object>> GetSessionStateForFrame { get; set; }
 
        #region Process lifetime management

        private String _pageKey;

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property provides the group to be displayed.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e == null) throw new ArgumentNullException("e");

            // Returning to a cached page through navigation shouldn't trigger state loading
            if (this._pageKey != null) return;

            var frameFacade = new FrameFacadeAdapter(this.Frame);
            var frameState = GetSessionStateForFrame(frameFacade);
            this._pageKey = "Page-" + frameFacade.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                // Clear existing state for forward navigation when adding a new page to the
                // navigation stack
                var nextPageKey = this._pageKey;
                int nextPageIndex = frameFacade.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                // Pass the navigation parameter to the new page
                this.LoadState(e.Parameter, null);
            }
            else
            {
                // Pass the navigation parameter and preserved page state to the page, using
                // the same strategy for loading suspended state and recreating pages discarded
                // from cache
                this.LoadState(e.Parameter, (Dictionary<String, Object>)frameState[this._pageKey]);
            }
        }

        /// <summary>
        /// Invoked when this page will no longer be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property provides the group to be displayed.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameFacade = new FrameFacadeAdapter(this.Frame);
            var frameState = GetSessionStateForFrame(frameFacade);
            var pageState = new Dictionary<String, Object>();
            this.SaveState(pageState);
            frameState[_pageKey] = pageState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session. This will be null the first time a page is visited.</param>
        protected virtual void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache. Values must conform to the serialization
        /// requirements of <see cref="SessionStateService.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected virtual void SaveState(Dictionary<String, Object> pageState)
        {
        }

        #endregion
    }
}
