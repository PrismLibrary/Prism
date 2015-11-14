

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using Prism.Properties;
using Microsoft.Practices.ServiceLocation;
using Prism.Common;
using System.Threading.Tasks;

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
            {
                throw new ArgumentNullException("serviceLocator");
            }

            if (regionNavigationContentLoader == null)
            {
                throw new ArgumentNullException("regionNavigationContentLoader");
            }

            if (journal == null)
            {
                throw new ArgumentNullException("journal");
            }

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
        /// Initiates navigation to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="navigationCallback">A callback to execute when the navigation request is completed.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is marshalled to callback")]
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback)
        {
            this.RequestNavigate(target, navigationCallback, null);
        }

        /// <summary>
        /// Initiates navigation to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="navigationCallback">A callback to execute when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (navigationCallback == null) throw new ArgumentNullException("navigationCallback");

            try
            {
                this.DoNavigate(target, navigationCallback, navigationParameters);
            }
            catch (Exception e)
            {
                var result = this.NotifyNavigationFailed(new NavigationContext(this, target), e);
                navigationCallback(result);
            }
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <returns>The result of the navigation request.</returns>
        public async Task<NavigationResult> RequestNavigateAsync(Uri target)
        {
            return await CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => this.RequestNavigate(target, callback));
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        /// <returns>The result of the navigation request.</returns>
        public async Task<NavigationResult> RequestNavigateAsync(Uri target, NavigationParameters navigationParameters)
        {
            return await CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => this.RequestNavigate(target, callback, navigationParameters));
        }

        #region synchronous implementation

        private void DoNavigate(Uri source, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (this.Region == null)
            {
                throw new InvalidOperationException(Resources.NavigationServiceHasNoRegion);
            }

            this.currentNavigationContext = new NavigationContext(this, source, navigationParameters);

            // starts querying the active views
            RequestCanNavigateFromOnCurrentlyActiveView(
                this.currentNavigationContext,
                navigationCallback,
                this.Region.ActiveViews.ToArray(),
                0);
        }

        private void RequestCanNavigateFromOnCurrentlyActiveView(
            NavigationContext navigationContext,
            Action<NavigationResult> navigationCallback,
            object[] activeViews,
            int currentViewIndex)
        {
            if (currentViewIndex < activeViews.Length)
            {
                var vetoingView = activeViews[currentViewIndex] as IConfirmNavigationRequest;
                if (vetoingView != null)
                {
                    // the current active view implements IConfirmNavigationRequest, request confirmation
                    // providing a callback to resume the navigation request
                    vetoingView.ConfirmNavigationRequest(
                        navigationContext,
                        canNavigate =>
                        {
                            if (this.currentNavigationContext == navigationContext && canNavigate)
                            {
                                RequestCanNavigateFromOnCurrentlyActiveViewModel(
                                    navigationContext,
                                    navigationCallback,
                                    activeViews,
                                    currentViewIndex);
                            }
                            else
                            {
                                navigationCallback(this.NotifyNavigationFailed(navigationContext, null));
                            }
                        });
                }
                else
                {
                    RequestCanNavigateFromOnCurrentlyActiveViewModel(
                        navigationContext,
                        navigationCallback,
                        activeViews,
                        currentViewIndex);
                }
            }
            else
            {
                ExecuteNavigation(navigationContext, activeViews, navigationCallback);
            }
        }

        private void RequestCanNavigateFromOnCurrentlyActiveViewModel(
            NavigationContext navigationContext,
            Action<NavigationResult> navigationCallback,
            object[] activeViews,
            int currentViewIndex)
        {
            var frameworkElement = activeViews[currentViewIndex] as FrameworkElement;

            if (frameworkElement != null)
            {
                var vetoingViewModel = frameworkElement.DataContext as IConfirmNavigationRequest;

                if (vetoingViewModel != null)
                {
                    // the data model for the current active view implements IConfirmNavigationRequest, request confirmation
                    // providing a callback to resume the navigation request
                    vetoingViewModel.ConfirmNavigationRequest(
                        navigationContext,
                        canNavigate =>
                        {
                            if (this.currentNavigationContext == navigationContext && canNavigate)
                            {
                                RequestCanNavigateFromOnCurrentlyActiveView(
                                    navigationContext,
                                    navigationCallback,
                                    activeViews,
                                    currentViewIndex + 1);
                            }
                            else
                            {
                                navigationCallback(this.NotifyNavigationFailed(navigationContext, null));
                            }
                        });

                    return;
                }
            }

            RequestCanNavigateFromOnCurrentlyActiveView(
                navigationContext,
                navigationCallback,
                activeViews,
                currentViewIndex + 1);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is marshalled to callback")]
        private void ExecuteNavigation(NavigationContext navigationContext, object[] activeViews, Action<NavigationResult> navigationCallback)
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

                navigationCallback(new NavigationResult(navigationContext, true));

                // Raise the navigated event when navigation is completed.
                this.RaiseNavigated(navigationContext);
            }
            catch (Exception e)
            {
                navigationCallback(this.NotifyNavigationFailed(navigationContext, e));
            }
        }

        #endregion

        #region async implementation

        private async Task<NavigationResult> DoNavigateAsync(Uri source, NavigationParameters navigationParameters)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (this.Region == null)
            {
                throw new InvalidOperationException(Resources.NavigationServiceHasNoRegion);
            }

            this.currentNavigationContext = new NavigationContext(this, source, navigationParameters);

            // starts querying the active views
            return await RequestCanNavigateFromOnCurrentlyActiveViewAsync(
                this.currentNavigationContext,
                this.Region.ActiveViews.ToArray(),
                0);
        }

        private async Task<NavigationResult> RequestCanNavigateFromOnCurrentlyActiveViewAsync(
            NavigationContext navigationContext,
            object[] activeViews,
            int currentViewIndex)
        {
            if (currentViewIndex < activeViews.Length)
            {
                var vetoingView = activeViews[currentViewIndex] as IConfirmNavigationRequest;
                if (vetoingView != null)
                {
                    // the current active view implements IConfirmNavigationRequest, request confirmation
                    // providing a callback to resume the navigation request
                    var canNavigate = await CallbackHelper.AwaitCallbackResult<bool>(callback =>
                        vetoingView.ConfirmNavigationRequest(navigationContext, callback));

                    if (this.currentNavigationContext == navigationContext && canNavigate)
                    {
                        return await RequestCanNavigateFromOnCurrentlyActiveViewModelAsync(
                            navigationContext,
                            activeViews,
                            currentViewIndex);
                    }
                    else
                    {
                        return this.NotifyNavigationFailed(navigationContext, null);
                    }
                }
                else
                {
                    return await RequestCanNavigateFromOnCurrentlyActiveViewModelAsync(
                        navigationContext,
                        activeViews,
                        currentViewIndex);
                }
            }
            else
            {
                return ExecuteNavigation(navigationContext, activeViews);
            }
        }

        private async Task<NavigationResult> RequestCanNavigateFromOnCurrentlyActiveViewModelAsync(
            NavigationContext navigationContext,
            object[] activeViews,
            int currentViewIndex)
        {
            var frameworkElement = activeViews[currentViewIndex] as FrameworkElement;

            if (frameworkElement != null)
            {
                var vetoingViewModel = frameworkElement.DataContext as IConfirmNavigationRequest;

                if (vetoingViewModel != null)
                {
                    // the data model for the current active view implements IConfirmNavigationRequest, request confirmation
                    // providing a callback to resume the navigation request
                    var canNavigate = await CallbackHelper.AwaitCallbackResult<bool>(callback =>
                        vetoingViewModel.ConfirmNavigationRequest(navigationContext, callback));

                    if (this.currentNavigationContext == navigationContext && canNavigate)
                    {
                        return await RequestCanNavigateFromOnCurrentlyActiveViewAsync(
                            navigationContext,
                            activeViews,
                            currentViewIndex + 1);
                    }
                    else
                    {
                        return this.NotifyNavigationFailed(navigationContext, null);
                    }
                }
            }

            return await RequestCanNavigateFromOnCurrentlyActiveViewAsync(
                navigationContext,
                activeViews,
                currentViewIndex + 1);
        }

        #endregion

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
