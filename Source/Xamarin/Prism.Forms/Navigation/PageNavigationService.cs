using Prism.Common;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reflection;

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
                throw;
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
            var nextPageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
            bool useReverse = UseReverseNavigation(currentPage, nextPageType);
            if (!useReverse)
            {
                var nextPage = CreatePageFromSegment(nextSegment);

                await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);

                await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
                {
                    await DoPush(currentPage, nextPage, useModalNavigation, animated);
                });
            }
            else
            {
                await UseReverseNavigation(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
        }

        protected virtual async Task ProcessNavigationForNavigationPage(NavigationPage currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            if (currentPage.Navigation.NavigationStack.Count == 0)
            {
                await UseReverseNavigation(currentPage, nextSegment, segments, parameters, false, animated);
                return;
            }

            var clearNavigationStack = GetClearNavigationPageNavigationStack(currentPage);
            var isEmptyOfNavigationStack = currentPage.Navigation.NavigationStack.Count == 0;

            List<Page> destroyPages;
            if (clearNavigationStack && !isEmptyOfNavigationStack)
            {
                destroyPages = currentPage.Navigation.NavigationStack.ToList();
                destroyPages.Reverse();

                await currentPage.Navigation.PopToRootAsync(false);
            }
            else
            {
                destroyPages = new List<Page>();
            }

            var topPage = currentPage.Navigation.NavigationStack.LastOrDefault();
            var nextPageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
            if (topPage?.GetType() == nextPageType)
            {
                if (clearNavigationStack)
                    destroyPages.Remove(destroyPages.Last());

                await UseReverseNavigation(topPage, null, segments, parameters, false, animated);
                await DoNavigateAction(topPage, nextSegment, topPage, parameters);
            }
            else
            {
                await UseReverseNavigation(currentPage, nextSegment, segments, parameters, false, animated);

                if (clearNavigationStack && !isEmptyOfNavigationStack)
                    currentPage.Navigation.RemovePage(topPage);
            }

            foreach (var destroyPage in destroyPages)
            {
                PageUtilities.DestroyPage(destroyPage);
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

            var detail = currentPage.Detail;
            if (detail == null)
            {
                var newDetail = CreatePageFromSegment(nextSegment);
                await ProcessNavigation(newDetail, segments, parameters, useModalNavigation, animated);
                await DoNavigateAction(null, nextSegment, newDetail, parameters, onNavigationActionCompleted: () =>
                {
                    currentPage.IsPresented = isPresented;
                    currentPage.Detail = newDetail;
                });
                return;
            }

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

        protected static bool UseReverseNavigation(Page currentPage, Type nextPageType)
        {
            return currentPage?.Parent is NavigationPage && IsSameOrSubclassOf<ContentPage>(nextPageType);
        }

        public static bool IsSameOrSubclassOf<T>(Type potentialDescendant)
        {
            Type potentialBase = typeof(T);
            return potentialDescendant.GetTypeInfo().IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }

        protected virtual async Task UseReverseNavigation(Page currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var navigationStack = new Stack<string>();

            if (!String.IsNullOrWhiteSpace(nextSegment))
                navigationStack.Push(nextSegment);

            var illegalSegments = new Queue<string>();

            bool illegalPageFound = false;
            foreach (var item in segments)
            {
                //if we run itno an illegal page, we need to create new navigation segments to properly handle the deep link
                if (illegalPageFound)
                {
                    illegalSegments.Enqueue(item);
                    continue;
                }

                var pageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(item));
                if (IsSameOrSubclassOf<MasterDetailPage>(pageType))
                {
                    illegalSegments.Enqueue(item);
                    illegalPageFound = true;
                }
                else
                {
                    navigationStack.Push(item);
                }
            }

            var pageOffset = currentPage.Navigation.NavigationStack.Count;
            if (currentPage.Navigation.NavigationStack.Count > 2)
                pageOffset = currentPage.Navigation.NavigationStack.Count - 1;

            bool insertBefore = false;
            while (navigationStack.Count > 0)
            {
                var segment = navigationStack.Pop();
                var nextPage = CreatePageFromSegment(segment);
                await DoNavigateAction(currentPage, segment, nextPage, parameters, async () =>
                {
                    await DoPush(currentPage, nextPage, useModalNavigation, animated, insertBefore, pageOffset);
                });
                insertBefore = true;
            }

            //if an illegal page is found, we force a Modal navigation
            if (illegalSegments.Count > 0)
                await ProcessNavigation(currentPage.Navigation.NavigationStack.Last(), illegalSegments, parameters, true, animated);
        }

        protected virtual Task DoPush(Page currentPage, Page page, bool? useModalNavigation, bool animated, bool insertBeforeLast = false, int navigationOffset = 0)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            if (currentPage == null)
            {
                _applicationProvider.MainPage = page;
                return Task.FromResult<object>(null);
            }
            else
            {
                bool useModalForPush = UseModalNavigation(currentPage, useModalNavigation);

                if (useModalForPush)
                {
                    return currentPage.Navigation.PushModalAsync(page, animated);
                }
                else
                {
                    if (insertBeforeLast)
                    {
                        return InsertPageBefore(currentPage, page, navigationOffset);
                    }
                    else
                    {
                        return currentPage.Navigation.PushAsync(page, animated);
                    }
                }

            }
        }

        protected virtual Task InsertPageBefore(Page currentPage, Page page, int pageOffset)
        {
            var navigationPage = currentPage.Parent as NavigationPage;
            var firstPage = currentPage.Navigation.NavigationStack.Skip(pageOffset).FirstOrDefault();
            currentPage.Navigation.InsertPageBefore(page, firstPage);
            return Task.FromResult(true);
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
