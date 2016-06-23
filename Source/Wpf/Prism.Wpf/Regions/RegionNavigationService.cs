

using Microsoft.Practices.ServiceLocation;
using Prism.Common;
using Prism.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Prism.Regions
{
    /// <summary>
    /// Provides navigation for regions.
    /// </summary>
    public class RegionNavigationService : IRegionNavigationService
    {
        private readonly IServiceLocator serviceLocator;
        private readonly IRegionNavigationContentLoader regionNavigationContentLoader;
        private IRegionNavigationJournal journal;
        private NavigationContext currentNavigationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationService"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="regionNavigationContentLoader">The navigation target handler.</param>
        /// <param name="journal">The journal.</param>
        public RegionNavigationService(IServiceLocator serviceLocator, IRegionNavigationContentLoader regionNavigationContentLoader, IRegionNavigationJournal journal)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException(nameof(serviceLocator));

            if (regionNavigationContentLoader == null)
                throw new ArgumentNullException(nameof(regionNavigationContentLoader));

            if (journal == null)
                throw new ArgumentNullException(nameof(journal));

            this.serviceLocator = serviceLocator;
            this.regionNavigationContentLoader = regionNavigationContentLoader;
            this.journal = journal;
            this.journal.NavigationTarget = this;
        }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>The region.</value>
        public IRegion Region { get; set; }

        /// <summary>
        /// Gets the journal.
        /// </summary>
        /// <value>The journal.</value>
        public IRegionNavigationJournal Journal
        {
            get
            {
                return this.journal;
            }
        }

        /// <summary>
        /// Raised when the region is about to be navigated to content.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigating;

        private void RaiseNavigating(NavigationContext navigationContext)
        {
            if (this.Navigating != null)
            {
                this.Navigating(this, new RegionNavigationEventArgs(navigationContext));
            }
        }

        /// <summary>
        /// Raised when the region is navigated to content.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigated;

        private void RaiseNavigated(NavigationContext navigationContext)
        {
            if (this.Navigated != null)
            {
                this.Navigated(this, new RegionNavigationEventArgs(navigationContext));
            }
        }

        /// <summary>
        /// Raised when a navigation request fails.
        /// </summary>
        public event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;

        private void RaiseNavigationFailed(NavigationContext navigationContext, Exception error)
        {
            if (this.NavigationFailed != null)
            {
                this.NavigationFailed(this, new RegionNavigationFailedEventArgs(navigationContext, error));
            }
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the 
        /// <see cref="NavigationAsyncExtensions"/> class.
        /// </remarks>
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback)
        {
            this.RequestNavigate(target, navigationCallback, null);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the 
        /// <see cref="NavigationAsyncExtensions"/> class.
        /// </remarks>
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            RequestNavigateImpl(target, navigationCallback, navigationParameters);
        }

        private async void RequestNavigateImpl(Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            NavigationResult result = await this.RequestNavigateAsync(target, navigationParameters);
            navigationCallback(result);
        }

        /// <summary>
        /// Initiates navigation to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        /// <returns>The navigation result.</returns>
        private async Task<NavigationResult> RequestNavigateAsync(Uri target, NavigationParameters navigationParameters)
        {
            try
            {
                return await this.DoNavigateAsync(target, navigationParameters);
            }
            catch (Exception e)
            {
                return this.NotifyNavigationFailed(new NavigationContext(this, target), e);
            }
        }

        private async Task<NavigationResult> DoNavigateAsync(Uri source, NavigationParameters navigationParameters)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (this.Region == null)
                throw new InvalidOperationException(Resources.NavigationServiceHasNoRegion);

            this.currentNavigationContext = new NavigationContext(this, source, navigationParameters);

            NavigationContext navigationContext = this.currentNavigationContext;

            object[] activeViews = this.Region.ActiveViews.ToArray();

            foreach (object view in activeViews)
            {
                var vetoingView = MvvmHelpers.GetImplementerFromViewOrViewModel<IConfirmNavigationRequestAsync>(view);

                if (vetoingView != null)
                {
                    // the current active view implements IConfirmNavigationRequestAsync, request confirmation
                    bool canNavigate = await vetoingView.ConfirmNavigationRequestAsync(navigationContext);

                    if (!(this.currentNavigationContext == navigationContext && canNavigate))
                    {
                        return this.NotifyNavigationFailed(navigationContext, null);
                    }
                }

                var vetoingViewOld = MvvmHelpers.GetImplementerFromViewOrViewModel<IConfirmNavigationRequest>(view);
                
                if (vetoingViewOld != null)
                {
                    // the current active view implements IConfirmNavigationRequest, request confirmation
                    // providing a callback to resume the navigation request
                    bool canNavigate = await CallbackHelper.AwaitCallbackResult<bool>(callback => vetoingViewOld.ConfirmNavigationRequest(navigationContext, callback));

                    if (!(this.currentNavigationContext == navigationContext && canNavigate))
                    {
                        return this.NotifyNavigationFailed(navigationContext, null);
                    }
                }
            }

            return ExecuteNavigation(navigationContext, activeViews);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is marshalled to callback")]
        private NavigationResult ExecuteNavigation(NavigationContext navigationContext, object[] activeViews)
        {
            try
            {
                NotifyActiveViewsNavigatingFrom(navigationContext, activeViews);

                object view = this.regionNavigationContentLoader.LoadContent(this.Region, navigationContext);

                // Raise the navigating event just before activing the view.
                this.RaiseNavigating(navigationContext);

                this.Region.Activate(view);

                // Update the navigation journal before notifying others of navigaton
                IRegionNavigationJournalEntry journalEntry = this.serviceLocator.GetInstance<IRegionNavigationJournalEntry>();
                journalEntry.Uri = navigationContext.Uri;
                journalEntry.Parameters = navigationContext.Parameters;
                this.journal.RecordNavigation(journalEntry);

                // The view can be informed of navigation
                Action<INavigationAware> action = (n) => n.OnNavigatedTo(navigationContext);
                MvvmHelpers.ViewAndViewModelAction(view, action);

                // Raise the navigated event when navigation is completed.
                this.RaiseNavigated(navigationContext);

                return new NavigationResult(navigationContext, true);
            }
            catch (Exception e)
            {
                return this.NotifyNavigationFailed(navigationContext, e);
            }
        }

        private NavigationResult NotifyNavigationFailed(NavigationContext navigationContext, Exception e)
        {
            var navigationResult =
                e != null ? new NavigationResult(navigationContext, e) : new NavigationResult(navigationContext, false);

            this.RaiseNavigationFailed(navigationContext, e);

            return navigationResult;
        }

        private static void NotifyActiveViewsNavigatingFrom(NavigationContext navigationContext, object[] activeViews)
        {
            InvokeOnNavigationAwareElements(activeViews, (n) => n.OnNavigatedFrom(navigationContext));
        }

        private static void InvokeOnNavigationAwareElements(IEnumerable<object> items, Action<INavigationAware> invocation)
        {
            foreach (var item in items)
            {
                MvvmHelpers.ViewAndViewModelAction(item, invocation);
            }
        }
    }
}
