using Prism.Common;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides page based navigation for ViewModels.
    /// </summary>
    public abstract class PageNavigationService : INavigationService, IPageAware
    {
        //not sure I like this static property, think about this a little more
        protected internal static PageNavigationSource NavigationSource { get; protected set; } = PageNavigationSource.Device;

        protected IApplicationProvider _applicationProvider;
        protected ILoggerFacade _logger;
        
        protected Page _page;
        Page IPageAware.Page
        {
            get { return _page; }
            set { _page = value; }
        }

        protected PageNavigationService(IApplicationProvider applicationProvider, ILoggerFacade logger)
        {
            _applicationProvider = applicationProvider;
            _logger = logger;
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
        public virtual async Task<bool> GoBackAsync(NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            try
            {
                NavigationSource = PageNavigationSource.NavigationService;

                var page = GetCurrentPage();
                var segmentParameters = UriParsingHelper.GetSegmentParameters(null, parameters);
                segmentParameters.Add(KnownNavigationParameters.NavigationMode, NavigationMode.Back);

                var canNavigate = await PageUtilities.CanNavigateAsync(page, segmentParameters);
                if (!canNavigate)
                    return false;                

                bool useModalForDoPop = UseModalNavigation(page, useModalNavigation);
                Page previousPage = PageUtilities.GetOnNavigatedToTarget(page, _applicationProvider.MainPage, useModalForDoPop);

                PageUtilities.OnNavigatingTo(previousPage, segmentParameters);

                var poppedPage = await DoPop(page.Navigation, useModalForDoPop, animated);
                if (poppedPage != null)
                {
                    PageUtilities.OnNavigatedFrom(page, segmentParameters);
                    PageUtilities.OnNavigatedTo(previousPage, segmentParameters);
                    PageUtilities.DestroyPage(poppedPage);
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), Category.Exception, Priority.High);
                return false;
            }
            finally
            {
                NavigationSource = PageNavigationSource.Device;
            }

            return false;
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        public virtual Task NavigateAsync(string name, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            return NavigateAsync(UriParsingHelper.Parse(name), parameters, useModalNavigation, animated);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// Navigate(new Uri("MainPage?id=3&name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        public virtual Task NavigateAsync(Uri uri, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            try
            {
                NavigationSource = PageNavigationSource.NavigationService;

                var navigationSegments = UriParsingHelper.GetUriSegments(uri);

                if (uri.IsAbsoluteUri)
                    return ProcessNavigationForAbsoulteUri(navigationSegments, parameters, useModalNavigation, animated);
                else
                    return ProcessNavigation(GetCurrentPage(), navigationSegments, parameters, useModalNavigation, animated);
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), Category.Exception, Priority.High);
                return Task.FromResult<object>(null);
            }
            finally
            {
                NavigationSource = PageNavigationSource.Device;
            }
        }

        protected virtual async Task ProcessNavigation(Page currentPage, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            if (segments.Count == 0)
                return;

            var nextSegment = segments.Dequeue();

            if (currentPage == null)
            {
                await ProcessNavigationForRootPage(nextSegment, segments, parameters, useModalNavigation, animated);
                return;
            }

            if (currentPage is ContentPage)
            {
                await ProcessNavigationForContentPage(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
            else if (currentPage is NavigationPage)
            {
                await ProcessNavigationForNavigationPage((NavigationPage)currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
            else if (currentPage is TabbedPage)
            {
                await ProcessNavigationForTabbedPage((TabbedPage)currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
            else if (currentPage is CarouselPage)
            {
                await ProcessNavigationForCarouselPage((CarouselPage)currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
            else if (currentPage is MasterDetailPage)
            {
                await ProcessNavigationForMasterDetailPage((MasterDetailPage)currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
        }

        protected virtual Task ProcessNavigationForAbsoulteUri(Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return ProcessNavigation(null, segments, parameters, useModalNavigation, animated);
        }

        protected virtual async Task ProcessNavigationForRootPage(string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextPage = CreatePageFromSegment(nextSegment);

            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);

            var currentPage = _applicationProvider.MainPage;
            var modalStack = currentPage?.Navigation.ModalStack.ToList();
            await DoNavigateAction(GetCurrentPage(), nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(null, nextPage, useModalNavigation, animated);
            });
            if (currentPage != null)
            {
                PageUtilities.DestroyWithModalStack(currentPage, modalStack);
            }
        }

        protected virtual async Task ProcessNavigationForContentPage(Page currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextPage = CreatePageFromSegment(nextSegment);

            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);

            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated);
            });
        }

        protected virtual async Task ProcessNavigationForNavigationPage(NavigationPage currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            if (currentPage.Navigation.NavigationStack.Count == 0)
            {
                var newRoot = CreatePageFromSegment(nextSegment);
                await ProcessNavigation(newRoot, segments, parameters, false, animated);
                await DoNavigateAction(currentPage, nextSegment, newRoot, parameters, async () =>
                {
                    await DoPush(currentPage, newRoot, false, animated);
                });
                return;
            }

            bool clearNavStack = GetClearNavigationPageNavigationStack(currentPage);

            var currentNavRoot = currentPage.Navigation.NavigationStack[0];
            var nextPageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
            if (currentNavRoot.GetType() == nextPageType)
            {
                if (clearNavStack && currentPage.Navigation.NavigationStack.Count > 1)
                    await currentPage.Navigation.PopToRootAsync(false);

                await ProcessNavigation(currentNavRoot, segments, parameters, false, animated);
                await DoNavigateAction(currentNavRoot, nextSegment, currentNavRoot, parameters);
                return;
            }
            else
            {
                if (clearNavStack && currentPage.Navigation.NavigationStack.Count > 1)
                    await currentPage.Navigation.PopToRootAsync(false);

                var newRoot = CreatePageFromSegment(nextSegment);
                await ProcessNavigation(newRoot, segments, parameters, false, animated);

                await DoNavigateAction(currentNavRoot, nextSegment, newRoot, parameters, async () =>
                {
                    var push = DoPush(currentPage, newRoot, false, animated);

                    if (clearNavStack && currentPage.Navigation.NavigationStack.Count > 1)
                    {
                        currentPage.Navigation.RemovePage(currentNavRoot);
                        PageUtilities.DestroyPage(currentNavRoot);
                    }

                    await push;
                });
                return;
            }
        }

        protected virtual async Task ProcessNavigationForTabbedPage(TabbedPage currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextSegmentType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
            foreach (var child in currentPage.Children)
            {
                if (child.GetType() != nextSegmentType)
                    continue;

                await ProcessNavigation(child, segments, parameters, useModalNavigation, animated);
                await DoNavigateAction(null, nextSegment, child, parameters, onNavigationActionCompleted: () =>
                {
                    currentPage.CurrentPage = child;
                });
                return;
            }

            var nextPage = CreatePageFromSegment(nextSegment);
            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated);
            });
        }

        protected virtual async Task ProcessNavigationForCarouselPage(CarouselPage currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextSegmentType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
            foreach (var child in currentPage.Children)
            {
                if (child.GetType() != nextSegmentType)
                    continue;

                await ProcessNavigation(child, segments, parameters, useModalNavigation, animated);
                await DoNavigateAction(null, nextSegment, child, parameters, onNavigationActionCompleted: () =>
                {
                    currentPage.CurrentPage = child;
                });
                return;
            }

            var nextPage = CreatePageFromSegment(nextSegment);
            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated);
            });
        }

        protected virtual async Task ProcessNavigationForMasterDetailPage(MasterDetailPage currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            bool isPresented = GetMasterDetailPageIsPresented(currentPage);

            if (useModalNavigation.HasValue && useModalNavigation.Value)
            {
                var nextPage = CreatePageFromSegment(nextSegment);
                await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
                await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
                {
                    currentPage.IsPresented = isPresented;
                    await DoPush(currentPage, nextPage, true, animated);
                });
                return;
            }

            var detail = currentPage.Detail;
            if (detail == null)
            {
                var newDetail = CreatePageFromSegment(nextSegment);
                await ProcessNavigation(newDetail, segments, parameters, newDetail is NavigationPage ? false : true, animated);
                await DoNavigateAction(null, nextSegment, newDetail, parameters, onNavigationActionCompleted: () =>
                {
                    currentPage.IsPresented = isPresented;
                    currentPage.Detail = newDetail;
                });
                return;
            }

            var nextSegmentType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
            if (detail.GetType() == nextSegmentType)
            {
                await ProcessNavigation(detail, segments, parameters, useModalNavigation, animated);
                await DoNavigateAction(null, nextSegment, detail, parameters, onNavigationActionCompleted: () =>
                 {
                     currentPage.IsPresented = isPresented;
                 });
                return;
            }
            else
            {
                var newDetail = CreatePageFromSegment(nextSegment);
                await ProcessNavigation(newDetail, segments, parameters, newDetail is NavigationPage ? false : true, animated);
                await DoNavigateAction(detail, nextSegment, newDetail, parameters, onNavigationActionCompleted: () =>
                {
                    currentPage.IsPresented = isPresented;
                    currentPage.Detail = newDetail;
                    PageUtilities.DestroyPage(detail);
                });
                return;
            }
        }

        protected static bool GetMasterDetailPageIsPresented(MasterDetailPage page)
        {
            var iMasterDetailPage = page as IMasterDetailPageOptions;
            if (iMasterDetailPage != null)
                return iMasterDetailPage.IsPresentedAfterNavigation;

            var iMasterDetailPageBindingContext = page.BindingContext as IMasterDetailPageOptions;
            if (iMasterDetailPageBindingContext != null)
                return iMasterDetailPageBindingContext.IsPresentedAfterNavigation;

            return false;
        }

        protected static bool GetClearNavigationPageNavigationStack(NavigationPage page)
        {
            var iNavigationPage = page as INavigationPageOptions;
            if (iNavigationPage != null)
                return iNavigationPage.ClearNavigationStackOnNavigation;

            var iNavigationPageBindingContext = page.BindingContext as INavigationPageOptions;
            if (iNavigationPageBindingContext != null)
                return iNavigationPageBindingContext.ClearNavigationStackOnNavigation;

            return true;
        }

        protected static async Task DoNavigateAction(Page fromPage, string toSegment, Page toPage, NavigationParameters parameters, Func<Task> navigationAction = null, Action onNavigationActionCompleted = null)
        {
            var segmentParameters = UriParsingHelper.GetSegmentParameters(toSegment, parameters);
            segmentParameters.Add(KnownNavigationParameters.NavigationMode, NavigationMode.New);

            var canNavigate = await PageUtilities.CanNavigateAsync(fromPage, segmentParameters);
            if (!canNavigate)
                return;

            PageUtilities.OnNavigatingTo(toPage, segmentParameters);

            if (navigationAction != null)
                await navigationAction();

            PageUtilities.OnNavigatedFrom(fromPage, segmentParameters);

            onNavigationActionCompleted?.Invoke();

            PageUtilities.OnNavigatedTo(toPage, segmentParameters);
        }

        protected abstract Page CreatePage(string segmentName);

        protected virtual Page CreatePageFromSegment(string segment)
        {
            try
            {
                var segmentName = UriParsingHelper.GetSegmentName(segment);
                var page = CreatePage(segmentName);
                if (page == null)
                    throw new NullReferenceException(string.Format("{0} could not be created. Please make sure you have registered {0} for navigation.", segmentName));

                SetAutowireViewModelOnPage(page);
                ApplyPageBehaviors(page);

                return page;
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), Category.Exception, Priority.High);
                throw;
            }
        }

        protected virtual void ApplyPageBehaviors(Page page)
        {
            if (page is NavigationPage)
            {
                page.Behaviors.Add(new Behaviors.NavigationPageSystemGoBackBehavior());
            }
            else if (page is TabbedPage)
            {
                page.Behaviors.Add(new Behaviors.TabbedPageActiveAwareBehavior());
            }
            else if (page is CarouselPage)
            {
                page.Behaviors.Add(new Behaviors.CarouselPageActiveAwareBehavior());
            }
        }

        protected void SetAutowireViewModelOnPage(Page page)
        {
            var vmlResult = Mvvm.ViewModelLocator.GetAutowireViewModel(page);
            if (vmlResult == null)
                Mvvm.ViewModelLocator.SetAutowireViewModel(page, true);
        }

        protected static bool HasNavigationPageParent(Page page)
        {
            return page?.Parent != null && page?.Parent is NavigationPage;
        }

        protected static bool UseModalNavigation(Page currentPage, bool? useModalNavigationDefault)
        {
            bool useModalNavigation = true;

            if (useModalNavigationDefault.HasValue)
                useModalNavigation = useModalNavigationDefault.Value;
            else
                useModalNavigation = !HasNavigationPageParent(currentPage);

            return useModalNavigation;
        }

        protected virtual async Task DoPush(Page currentPage, Page page, bool? useModalNavigation, bool animated)
        {
            if (page == null)
                return;

            if (currentPage == null)
            {
                _applicationProvider.MainPage = page;
            }
            else
            {
                bool useModalForPush = UseModalNavigation(currentPage, useModalNavigation);

                if (useModalForPush)
                    await currentPage.Navigation.PushModalAsync(page, animated);
                else
                    await currentPage.Navigation.PushAsync(page, animated);
            }
        }

        protected virtual Task<Page> DoPop(INavigation navigation, bool useModalNavigation, bool animated)
        {
            if (useModalNavigation)
                return navigation.PopModalAsync(animated);
            else
                return navigation.PopAsync(animated);
        }

        protected virtual Page GetCurrentPage()
        {
            return _page != null ? _page : _applicationProvider.MainPage;
        }
    }
}
