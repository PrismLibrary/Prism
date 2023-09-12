using System;
using System.Linq;
using Prism.Common;
using Prism.Ioc;
using Prism.Properties;
using Xamarin.Forms;

namespace Prism.Navigation.Regions.Navigation
{
    /// <summary>
    /// Provides navigation for regions.
    /// </summary>
    public class RegionNavigationService : IRegionNavigationService
    {
        private readonly IContainerProvider _container;
        private readonly IRegionNavigationContentLoader _regionNavigationContentLoader;
        private NavigationContext _currentNavigationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationService"/> class.
        /// </summary>
        /// <param name="container">The <see cref="IContainerExtension" />.</param>
        /// <param name="regionNavigationContentLoader">The navigation target handler.</param>
        /// <param name="journal">The journal.</param>
        public RegionNavigationService(IContainerExtension container, IRegionNavigationContentLoader regionNavigationContentLoader, IRegionNavigationJournal journal)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _regionNavigationContentLoader = regionNavigationContentLoader ?? throw new ArgumentNullException(nameof(regionNavigationContentLoader));
            Journal = journal ?? throw new ArgumentNullException(nameof(journal));
            Journal.NavigationTarget = this;
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
        public IRegionNavigationJournal Journal { get; private set; }

        /// <summary>
        /// Raised when the region is about to be navigated to content.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigating;

        private void RaiseNavigating(NavigationContext navigationContext)
        {
            Navigating?.Invoke(this, new RegionNavigationEventArgs(navigationContext));
        }

        /// <summary>
        /// Raised when the region is navigated to content.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigated;

        private void RaiseNavigated(NavigationContext navigationContext)
        {
            Navigated?.Invoke(this, new RegionNavigationEventArgs(navigationContext));
        }

        /// <summary>
        /// Raised when a navigation request fails.
        /// </summary>
        public event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;

        private void RaiseNavigationFailed(NavigationContext navigationContext, Exception error)
        {
            NavigationFailed?.Invoke(this, new RegionNavigationFailedEventArgs(navigationContext, error));
        }

        /// <summary>
        /// Initiates navigation to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="navigationCallback">A callback to execute when the navigation request is completed.</param>
        /// <param name="regionParameters">The navigation parameters specific to the navigation request.</param>
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, INavigationParameters regionParameters)
        {
            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            try
            {
                DoNavigate(target, navigationCallback, regionParameters);
            }
            catch (Exception e)
            {
                NotifyNavigationFailed(new NavigationContext(this, target), navigationCallback, e);
            }
        }

        private void DoNavigate(Uri source, Action<NavigationResult> navigationCallback, INavigationParameters regionParameters)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (Region == null)
                throw new InvalidOperationException(Resources.NavigationServiceHasNoRegion);

            _currentNavigationContext = new NavigationContext(this, source, regionParameters);

            // starts querying the active views
            RequestCanNavigateFromOnCurrentlyActiveView(
                _currentNavigationContext,
                navigationCallback,
                Region.ActiveViews.OfType<VisualElement>().ToArray(),
                0);
        }

        private void RequestCanNavigateFromOnCurrentlyActiveView(
            NavigationContext navigationContext,
            Action<NavigationResult> navigationCallback,
            VisualElement[] activeViews,
            int currentViewIndex)
        {
            if (currentViewIndex < activeViews.Length)
            {
                if (activeViews[currentViewIndex] is IConfirmNavigationRequest vetoingView)
                {
                    // the current active view implements IConfirmNavigationRequest, request confirmation
                    // providing a callback to resume the navigation request
                    vetoingView.ConfirmNavigationRequest(
                        navigationContext,
                        canNavigate =>
                        {
                            if (_currentNavigationContext == navigationContext && canNavigate)
                            {
                                RequestCanNavigateFromOnCurrentlyActiveViewModel(
                                    navigationContext,
                                    navigationCallback,
                                    activeViews,
                                    currentViewIndex);
                            }
                            else
                            {
                                NotifyNavigationFailed(navigationContext, navigationCallback, null);
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
            VisualElement[] activeViews,
            int currentViewIndex)
        {
            if (activeViews[currentViewIndex].BindingContext is IConfirmNavigationRequest vetoingViewModel)
            {
                // the data model for the current active view implements IConfirmNavigationRequest, request confirmation
                // providing a callback to resume the navigation request
                vetoingViewModel.ConfirmNavigationRequest(
                    navigationContext,
                    canNavigate =>
                    {
                        if (_currentNavigationContext == navigationContext && canNavigate)
                        {
                            RequestCanNavigateFromOnCurrentlyActiveView(
                                navigationContext,
                                navigationCallback,
                                activeViews,
                                currentViewIndex + 1);
                        }
                        else
                        {
                            NotifyNavigationFailed(navigationContext, navigationCallback, null);
                        }
                    });

                return;
            }

            RequestCanNavigateFromOnCurrentlyActiveView(
                navigationContext,
                navigationCallback,
                activeViews,
                currentViewIndex + 1);
        }

        private void ExecuteNavigation(NavigationContext navigationContext, object[] activeViews, Action<NavigationResult> navigationCallback)
        {
            try
            {
                NotifyActiveViewsNavigatingFrom(navigationContext, activeViews);

                var view = (VisualElement)_regionNavigationContentLoader.LoadContent(Region, navigationContext);

                // Raise the navigating event just before activating the view.
                RaiseNavigating(navigationContext);

                Region.Activate(view);

                // Update the navigation journal before notifying others of navigation
                IRegionNavigationJournalEntry journalEntry = _container.Resolve<IRegionNavigationJournalEntry>();
                journalEntry.Uri = navigationContext.Uri;
                journalEntry.Parameters = navigationContext.Parameters;

                bool persistInHistory = PersistInHistory(view);

                Journal.RecordNavigation(journalEntry, persistInHistory);

                // The view can be informed of navigation
                MvvmHelpers.OnNavigatedTo(view, navigationContext);

                navigationCallback(new NavigationResult(navigationContext, true));

                // Raise the navigated event when navigation is completed.
                RaiseNavigated(navigationContext);
            }
            catch (Exception e)
            {
                NotifyNavigationFailed(navigationContext, navigationCallback, e);
            }
        }

        private static bool PersistInHistory(object view)
        {
            bool persist = true;
            MvvmHelpers.ViewAndViewModelAction<IJournalAware>(view, ija => { persist &= ija.PersistInHistory(); });
            return persist;
        }

        private void NotifyNavigationFailed(NavigationContext navigationContext, Action<NavigationResult> navigationCallback, Exception e)
        {
            var navigationResult =
                e != null ? new NavigationResult(navigationContext, e) : new NavigationResult(navigationContext, false);

            navigationCallback(navigationResult);
            RaiseNavigationFailed(navigationContext, e);
        }

        private static void NotifyActiveViewsNavigatingFrom(NavigationContext navigationContext, object[] activeViews)
        {
            foreach (var item in activeViews)
            {
                MvvmHelpers.OnNavigatedFrom(item, navigationContext);
            }
        }
    }
}
