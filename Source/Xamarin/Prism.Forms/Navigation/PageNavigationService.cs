using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
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
    public class PageNavigationService : INavigationService, INavigateInternal, IPageAware
    {
        internal const string RemovePageRelativePath = "../";
        internal const string RemovePageInstruction = "__RemovePage/";
        internal const string RemovePageSegment = "__RemovePage";

        //not sure I like this static property, think about this a little more
        protected internal static PageNavigationSource NavigationSource { get; protected set; } = PageNavigationSource.Device;

        private readonly IContainerProvider _container;
        protected readonly IApplicationProvider _applicationProvider;
        protected readonly IPageBehaviorFactory _pageBehaviorFactory;
        protected readonly ILoggerFacade _logger;

        protected Page _page;
        Page IPageAware.Page
        {
            get { return _page; }
            set { _page = value; }
        }

        public PageNavigationService(IContainerExtension container, IApplicationProvider applicationProvider, IPageBehaviorFactory pageBehaviorFactory, ILoggerFacade logger)
        {
            _container = container;
            _applicationProvider = applicationProvider;
            _pageBehaviorFactory = pageBehaviorFactory;
            _logger = logger;
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
        public virtual Task<INavigationResult> GoBackAsync()
        {
            return GoBackAsync(null);
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
        public virtual Task<INavigationResult> GoBackAsync(INavigationParameters parameters)
        {
            return GoBackInternal(parameters, null, true);
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
        protected async virtual Task<INavigationResult> GoBackInternal(INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var result = new NavigationResult();

            try
            {
                NavigationSource = PageNavigationSource.NavigationService;

                var page = GetCurrentPage();
                var segmentParameters = UriParsingHelper.GetSegmentParameters(null, parameters);
                segmentParameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

                var canNavigate = await PageUtilities.CanNavigateAsync(page, segmentParameters);
                if (!canNavigate)
                {
                    result.Exception = new Exception($"IConfirmNavigation for {page} returned false");
                    return result;
                }

                bool useModalForDoPop = UseModalNavigation(page, useModalNavigation);
                Page previousPage = PageUtilities.GetOnNavigatedToTarget(page, _applicationProvider.MainPage, useModalForDoPop);

                PageUtilities.OnNavigatingTo(previousPage, segmentParameters);

                var poppedPage = await DoPop(page.Navigation, useModalForDoPop, animated);
                if (poppedPage != null)
                {
                    PageUtilities.OnNavigatedFrom(page, segmentParameters);
                    PageUtilities.OnNavigatedTo(previousPage, segmentParameters);
                    PageUtilities.DestroyPage(poppedPage);

                    result.Success = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString(), Category.Exception, Priority.High);
                result.Exception = ex;
                return result;
            }
            finally
            {
                NavigationSource = PageNavigationSource.Device;
            }

            result.Exception = new Exception("Unknown error occured.");
            return result;
        }

        /// <summary>
        /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
        /// </summary>
        /// <param name="navigationService">The INavigatinService instance</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <remarks>Only works when called from a View within a NavigationPage</remarks>
        protected async virtual Task<INavigationResult> GoBackToRootInternal(INavigationParameters parameters)
        {
            var result = new NavigationResult();
            try
            {
                if (parameters == null)
                    parameters = new NavigationParameters();

                parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

                var page = GetCurrentPage();
                var canNavigate = await PageUtilities.CanNavigateAsync(page, parameters);
                if (!canNavigate)
                {
                    result.Exception = new Exception($"IConfirmNavigation for {page} returned false");
                    return result;
                }

                List<Page> pagesToDestroy = page.Navigation.NavigationStack.ToList(); // get all pages to destroy
                pagesToDestroy.Reverse(); // destroy them in reverse order
                var root = pagesToDestroy.Last();
                pagesToDestroy.Remove(root); //don't destroy the root page

                PageUtilities.OnNavigatingTo(root, parameters);

                await page.Navigation.PopToRootAsync();

                foreach (var destroyPage in pagesToDestroy)
                {
                    PageUtilities.OnNavigatedFrom(destroyPage, parameters);
                    PageUtilities.DestroyPage(destroyPage);
                }

                PageUtilities.OnNavigatedTo(root, parameters);

                result.Success = true;
                return result;
            }
            catch (InvalidOperationException ex)
            {
                result.Exception = new Exception("GoBackToRootAsync can only be called when the calling Page is within a NavigationPage.", ex);
                return result;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                return result;
            }
        }

        Task<INavigationResult> INavigateInternal.GoBackInternal(INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return GoBackInternal(parameters, useModalNavigation, animated);
        }

        Task<INavigationResult> INavigateInternal.GoBackToRootInternal(INavigationParameters parameters)
        {
            return GoBackToRootInternal(parameters);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        public virtual Task<INavigationResult> NavigateAsync(string name)
        {
            return NavigateAsync(name, null);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        public virtual Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters)
        {
            return NavigateInternal(name, parameters, null, true);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        protected virtual Task<INavigationResult> NavigateInternal(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            if (name.StartsWith(RemovePageRelativePath))
                name = name.Replace(RemovePageRelativePath, RemovePageInstruction);

            return NavigateInternal(UriParsingHelper.Parse(name), parameters, useModalNavigation, animated);
        }

        Task<INavigationResult> INavigateInternal.NavigateInternal(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return NavigateInternal(name, parameters, useModalNavigation, animated);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// Navigate(new Uri("MainPage?id=3&name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        public virtual Task<INavigationResult> NavigateAsync(Uri uri)
        {
            return NavigateAsync(uri, null);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// Navigate(new Uri("MainPage?id=3&name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        public virtual Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters)
        {
            return NavigateInternal(uri, parameters, null, true);
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
        protected async virtual Task<INavigationResult> NavigateInternal(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var result = new NavigationResult();
            try
            {
                NavigationSource = PageNavigationSource.NavigationService;

                var navigationSegments = UriParsingHelper.GetUriSegments(uri);

                if (uri.IsAbsoluteUri)
                {
                    await ProcessNavigationForAbsoulteUri(navigationSegments, parameters, useModalNavigation, animated);
                    result.Success = true;
                    return result;
                }
                else
                {
                    await ProcessNavigation(GetCurrentPage(), navigationSegments, parameters, useModalNavigation, animated);
                    result.Success = true;
                    return result;
                }

            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString(), Category.Exception, Priority.High);
                result.Exception = ex;
                return result;
            }
            finally
            {
                NavigationSource = PageNavigationSource.Device;
            }
        }

        Task<INavigationResult> INavigateInternal.NavigateInternal(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return NavigateInternal(uri, parameters, useModalNavigation, animated);
        }

        protected virtual async Task ProcessNavigation(Page currentPage, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            if (segments.Count == 0)
                return;

            var nextSegment = segments.Dequeue();

            var pageParameters = UriParsingHelper.GetSegmentParameters(nextSegment);
            if (pageParameters.ContainsKey(KnownNavigationParameters.UseModalNavigation))
                useModalNavigation = pageParameters.GetValue<bool>(KnownNavigationParameters.UseModalNavigation);

            if (nextSegment == RemovePageSegment)
            {
                await ProcessNavigationForRemovePageSegments(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
                return;
            }

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

        protected virtual Task ProcessNavigationForRemovePageSegments(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            if (!PageUtilities.HasDirectNavigationPageParent(currentPage))
                throw new InvalidOperationException("Removing views using the relative '../' syntax while navigating is only supported within a NavigationPage");

            if (CanRemoveAndPush(segments))
                return RemoveAndPush(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            else
                return RemoveAndGoBack(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
        }

        bool CanRemoveAndPush(Queue<string> segments)
        {
            if (segments.All(x => x == RemovePageSegment))
                return false;
            else
                return true;
        }

        Task RemoveAndGoBack(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            List<Page> pagesToRemove = new List<Page>();

            var currentPageIndex = currentPage.Navigation.NavigationStack.Count;
            if (currentPage.Navigation.NavigationStack.Count > 0)
                currentPageIndex = currentPage.Navigation.NavigationStack.Count - 1;

            while (segments.Count != 0)
            {
                currentPageIndex -= 1;
                pagesToRemove.Add(currentPage.Navigation.NavigationStack[currentPageIndex]);
                nextSegment = segments.Dequeue();
            }

            RemovePagesFromNavigationPage(currentPage, pagesToRemove);

            return GoBackAsync(parameters);
        }

        async Task RemoveAndPush(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            List<Page> pagesToRemove = new List<Page>();
            pagesToRemove.Add(currentPage);

            var currentPageIndex = currentPage.Navigation.NavigationStack.Count;
            if (currentPage.Navigation.NavigationStack.Count > 0)
                currentPageIndex = currentPage.Navigation.NavigationStack.Count - 1;

            while (segments.Peek() == RemovePageSegment)
            {
                currentPageIndex -= 1;
                pagesToRemove.Add(currentPage.Navigation.NavigationStack[currentPageIndex]);
                nextSegment = segments.Dequeue();
            }

            await ProcessNavigation(currentPage, segments, parameters, useModalNavigation, animated);

            RemovePagesFromNavigationPage(currentPage, pagesToRemove);
        }

        private static void RemovePagesFromNavigationPage(Page currentPage, List<Page> pagesToRemove)
        {
            var navigationPage = (NavigationPage)currentPage.Parent;
            foreach (var page in pagesToRemove)
            {
                navigationPage.Navigation.RemovePage(page);
                PageUtilities.DestroyPage(page);
            }
        }

        protected virtual Task ProcessNavigationForAbsoulteUri(Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return ProcessNavigation(null, segments, parameters, useModalNavigation, animated);
        }

        protected virtual async Task ProcessNavigationForRootPage(string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
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

        protected virtual async Task ProcessNavigationForContentPage(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextPageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
            bool useReverse = UseReverseNavigation(currentPage, nextPageType) && !(useModalNavigation.HasValue && useModalNavigation.Value);
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

        protected virtual async Task ProcessNavigationForNavigationPage(NavigationPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
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

                if (segments.Count > 0)
                    await UseReverseNavigation(topPage, segments.Dequeue(), segments, parameters, false, animated);

                await DoNavigateAction(topPage, nextSegment, topPage, parameters, onNavigationActionCompleted: () =>
                {
                    if (nextSegment.Contains(KnownNavigationParameters.SelectedTab))
                    {
                        var segmentParams = UriParsingHelper.GetSegmentParameters(nextSegment);
                        SelectPageTab(topPage, segmentParams);
                    }
                });
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

        protected virtual async Task ProcessNavigationForTabbedPage(TabbedPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextPage = CreatePageFromSegment(nextSegment);
            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated);
            });
        }

        protected virtual async Task ProcessNavigationForCarouselPage(CarouselPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextPage = CreatePageFromSegment(nextSegment);
            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated);
            });
        }

        protected virtual async Task ProcessNavigationForMasterDetailPage(MasterDetailPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
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

        protected static async Task DoNavigateAction(Page fromPage, string toSegment, Page toPage, INavigationParameters parameters, Func<Task> navigationAction = null, Action onNavigationActionCompleted = null)
        {
            var segmentParameters = UriParsingHelper.GetSegmentParameters(toSegment, parameters);
            segmentParameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);

            var canNavigate = await PageUtilities.CanNavigateAsync(fromPage, segmentParameters);
            if (!canNavigate)
                return;

            OnNavigatingTo(toPage, segmentParameters);

            if (navigationAction != null)
                await navigationAction();

            OnNavigatedFrom(fromPage, segmentParameters);

            onNavigationActionCompleted?.Invoke();

            OnNavigatedTo(toPage, segmentParameters);
        }

        static void OnNavigatingTo(Page toPage, INavigationParameters parameters)
        {
            PageUtilities.OnNavigatingTo(toPage, parameters);

            if (toPage is TabbedPage tabbedPage)
            {
                foreach (var child in tabbedPage.Children)
                {
                    if (child is NavigationPage navigationPage)
                    {
                        PageUtilities.OnNavigatingTo(navigationPage.CurrentPage, parameters);
                    }
                    else
                    {
                        PageUtilities.OnNavigatingTo(child, parameters);
                    }
                }
            }
            else if (toPage is CarouselPage carouselPage)
            {
                foreach (var child in carouselPage.Children)
                {
                    PageUtilities.OnNavigatingTo(child, parameters);
                }
            }
        }

        private static void OnNavigatedTo(Page toPage, INavigationParameters parameters)
        {
            PageUtilities.OnNavigatedTo(toPage, parameters);

            if (toPage is TabbedPage tabbedPage)
            {
                if (tabbedPage.CurrentPage is NavigationPage navigationPage)
                {
                    PageUtilities.OnNavigatedTo(navigationPage.CurrentPage, parameters);
                }
                else
                {
                    if (tabbedPage.BindingContext != tabbedPage.CurrentPage.BindingContext)
                        PageUtilities.OnNavigatedTo(tabbedPage.CurrentPage, parameters);
                }
            }
            else if (toPage is CarouselPage carouselPage)
            {
                PageUtilities.OnNavigatedTo(carouselPage.CurrentPage, parameters);
            }
        }

        private static void OnNavigatedFrom(Page fromPage, INavigationParameters parameters)
        {
            PageUtilities.OnNavigatedFrom(fromPage, parameters);

            if (fromPage is TabbedPage tabbedPage)
            {
                if (tabbedPage.CurrentPage is NavigationPage navigationPage)
                {
                    PageUtilities.OnNavigatedFrom(navigationPage.CurrentPage, parameters);
                }
                else
                {
                    if (tabbedPage.BindingContext != tabbedPage.CurrentPage.BindingContext)
                        PageUtilities.OnNavigatedFrom(tabbedPage.CurrentPage, parameters);
                }
            }
            else if (fromPage is CarouselPage carouselPage)
            {
                PageUtilities.OnNavigatedFrom(carouselPage.CurrentPage, parameters);
            }
        }

        protected virtual Page CreatePage(string segmentName)
        {
            return _container.Resolve<object>(segmentName) as Page;
        }

        protected virtual Page CreatePageFromSegment(string segment)
        {
            try
            {
                var segmentName = UriParsingHelper.GetSegmentName(segment);
                var page = CreatePage(segmentName);
                if (page == null)
                    throw new NullReferenceException(string.Format("{0} could not be created. Please make sure you have registered {0} for navigation.", segmentName));

                PageUtilities.SetAutowireViewModelOnPage(page);
                _pageBehaviorFactory.ApplyPageBehaviors(page);
                ConfigurePages(page, segment);

                return page;
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), Category.Exception, Priority.High);
                throw;
            }
        }

        void ConfigurePages(Page page, string segment)
        {
            if (page is TabbedPage)
            {
                ConfigureTabbedPage((TabbedPage)page, segment);
            }
            else if (page is CarouselPage)
            {
                ConfigureCarouselPage((CarouselPage)page, segment);
            }
        }

        void ConfigureTabbedPage(TabbedPage tabbedPage, string segment)
        {
            foreach (var child in tabbedPage.Children)
            {
                PageUtilities.SetAutowireViewModelOnPage(child);
                _pageBehaviorFactory.ApplyPageBehaviors(child);
                if (child is NavigationPage navPage)
                {
                    PageUtilities.SetAutowireViewModelOnPage(navPage.CurrentPage);
                    _pageBehaviorFactory.ApplyPageBehaviors(navPage.CurrentPage);
                }
            }

            var parameters = UriParsingHelper.GetSegmentParameters(segment);

            var tabsToCreate = parameters.GetValues<string>(KnownNavigationParameters.CreateTab);
            if (tabsToCreate.Count() > 0)
            {
                foreach (var tabToCreate in tabsToCreate)
                {
                    //created tab can be a single view or a view nested in a NavigationPage with the syntax "NavigationPage|ViewToCreate"
                    var tabSegements = tabToCreate.Split('|');
                    if (tabSegements.Length > 1)
                    {
                        var navigationPage = CreatePageFromSegment(tabSegements[0]) as NavigationPage;
                        if (navigationPage != null)
                        {
                            var navigationPageChild = CreatePageFromSegment(tabSegements[1]);

                            navigationPage.PushAsync(navigationPageChild);

                            //when creating a NavigationPage w/ DI, a blank Page object is injected into the ctor. Let's remove it
                            if (navigationPage.Navigation.NavigationStack.Count > 1)
                                navigationPage.Navigation.RemovePage(navigationPage.Navigation.NavigationStack[0]);

                            //set the title because Xamarin doesn't do this for us.
                            navigationPage.Title = navigationPageChild.Title;
                            navigationPage.Icon = navigationPageChild.Icon;

                            tabbedPage.Children.Add(navigationPage);
                        }
                    }
                    else
                    {
                        var tab = CreatePageFromSegment(tabToCreate);
                        tabbedPage.Children.Add(tab);
                    }
                }
            }

            TabbedPageSelectTab(tabbedPage, parameters);
        }

        void ConfigureCarouselPage(CarouselPage carouselPage, string segment)
        {
            foreach (var child in carouselPage.Children)
            {
                PageUtilities.SetAutowireViewModelOnPage(child);
            }

            var parameters = UriParsingHelper.GetSegmentParameters(segment);

            CarouselPageSelectTab(carouselPage, parameters);
        }

        private static void SelectPageTab(Page page, INavigationParameters parameters)
        {
            if (page is TabbedPage tabbedPage)
            {
                TabbedPageSelectTab(tabbedPage, parameters);
            }
            else if (page is CarouselPage carouselPage)
            {
                CarouselPageSelectTab(carouselPage, parameters);
            }
        }

        private static void TabbedPageSelectTab(TabbedPage tabbedPage, INavigationParameters parameters)
        {
            var selectedTab = parameters?.GetValue<string>(KnownNavigationParameters.SelectedTab);
            if (!string.IsNullOrWhiteSpace(selectedTab))
            {
                var selectedTabType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(selectedTab));

                var childFound = false;
                foreach (var child in tabbedPage.Children)
                {
                    if (!childFound && child.GetType() == selectedTabType)
                    {
                        tabbedPage.CurrentPage = child;
                        childFound = true;
                    }

                    if (child is NavigationPage)
                    {
                        if (!childFound && ((NavigationPage)child).CurrentPage.GetType() == selectedTabType)
                        {
                            tabbedPage.CurrentPage = child;
                            childFound = true;
                        }
                    }
                }
            }
        }

        private static void CarouselPageSelectTab(CarouselPage carouselPage, INavigationParameters parameters)
        {
            var selectedTab = parameters?.GetValue<string>(KnownNavigationParameters.SelectedTab);
            if (!string.IsNullOrWhiteSpace(selectedTab))
            {
                var selectedTabType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(selectedTab));

                foreach (var child in carouselPage.Children)
                {
                    if (child.GetType() == selectedTabType)
                        carouselPage.CurrentPage = child;
                }
            }
        }

        protected virtual async Task UseReverseNavigation(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var navigationStack = new Stack<string>();

            if (!String.IsNullOrWhiteSpace(nextSegment))
                navigationStack.Push(nextSegment);

            var illegalSegments = new Queue<string>();

            bool illegalPageFound = false;
            foreach (var item in segments)
            {
                //if we run into an illegal page, we need to create new navigation segments to properly handle the deep link
                if (illegalPageFound)
                {
                    illegalSegments.Enqueue(item);
                    continue;
                }

                //if any page decide to go modal, we need to consider it and all pages after it an illegal page
                var pageParameters = UriParsingHelper.GetSegmentParameters(item);
                if (pageParameters.ContainsKey(KnownNavigationParameters.UseModalNavigation))
                {
                    if (pageParameters.GetValue<bool>(KnownNavigationParameters.UseModalNavigation))
                    {
                        illegalSegments.Enqueue(item);
                        illegalPageFound = true;
                    }
                    else
                    {
                        navigationStack.Push(item);
                    }
                }
                else
                {
                    var pageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(item));
                    if (PageUtilities.IsSameOrSubclassOf<MasterDetailPage>(pageType))
                    {
                        illegalSegments.Enqueue(item);
                        illegalPageFound = true;
                    }
                    else
                    {
                        navigationStack.Push(item);
                    }
                }
            }

            var pageOffset = currentPage.Navigation.NavigationStack.Count;
            if (currentPage.Navigation.NavigationStack.Count > 2)
                pageOffset = currentPage.Navigation.NavigationStack.Count - 1;

            var onNavigatedFromTarget = currentPage;
            if (currentPage is NavigationPage navPage && navPage.CurrentPage != null)
                onNavigatedFromTarget = navPage.CurrentPage;

            bool insertBefore = false;
            while (navigationStack.Count > 0)
            {
                var segment = navigationStack.Pop();
                var nextPage = CreatePageFromSegment(segment);
                await DoNavigateAction(onNavigatedFromTarget, segment, nextPage, parameters, async () =>
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

        internal static bool UseModalNavigation(Page currentPage, bool? useModalNavigationDefault)
        {
            bool useModalNavigation = true;

            if (useModalNavigationDefault.HasValue)
                useModalNavigation = useModalNavigationDefault.Value;
            else if (currentPage is NavigationPage)
                useModalNavigation = false;
            else
                useModalNavigation = !PageUtilities.HasNavigationPageParent(currentPage);

            return useModalNavigation;
        }

        internal static bool UseReverseNavigation(Page currentPage, Type nextPageType)
        {
            return PageUtilities.HasNavigationPageParent(currentPage) && PageUtilities.IsSameOrSubclassOf<ContentPage>(nextPageType);
        }
    }
}
