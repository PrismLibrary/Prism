using Prism.Common;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides page based navigation for ViewModels.
    /// </summary>
    public abstract class PageNavigationService : INavigationService, IPageAware
    {
        protected IApplicationProvider _applicationProvider;
        protected ILoggerFacade _logger;

        private Page _page;
        Page IPageAware.Page
        {
            get { return _page; }
            set { _page = value; }
        }

        public PageNavigationService(IApplicationProvider applicationProvider, ILoggerFacade logger)
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
                var page = GetCurrentPage();
                var segmentParameters = GetSegmentParameters(null, parameters);

                var canNavigate = await CanNavigateAsync(page, segmentParameters);
                if (!canNavigate)
                    return false;

                bool useModalForDoPop = UseModalNavigation(page, useModalNavigation);
                Page previousPage = GetOnNavigatedToTarget(page, useModalForDoPop);

                OnNavigatedFrom(page, segmentParameters);

                var poppedPage = await DoPop(page.Navigation, useModalForDoPop, animated);
                if (poppedPage != null)
                {
                    OnNavigatedTo(previousPage, segmentParameters);
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), Category.Exception, Priority.High);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <typeparamref name="TViewModel"/>.
        /// </summary>
        /// <typeparam name="TViewModel">The type which will be used to identify the name of the navigation target.</typeparam>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        public virtual Task NavigateAsync<TViewModel>(NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
            where TViewModel : BindableBase
        {
            return NavigateAsync(typeof(TViewModel).FullName, parameters, useModalNavigation, animated);
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
            return NavigateAsync(new Uri(name, UriKind.RelativeOrAbsolute), parameters, useModalNavigation, animated);
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
            var navigationSegments = UriParsingHelper.GetUriSegments(uri);

            if (uri.IsAbsoluteUri)
                return ProcessNavigationForAbsoulteUri(navigationSegments, parameters, useModalNavigation, animated);
            else
                return ProcessNavigation(GetCurrentPage(), navigationSegments, parameters, useModalNavigation, animated);
        }

        async Task ProcessNavigation(Page currentPage, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
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

        async Task ProcessNavigationForAbsoulteUri(Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            await ProcessNavigation(null, segments, parameters, useModalNavigation, animated);
        }

        async Task ProcessNavigationForRootPage(string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextPage = CreatePageFromSegment(nextSegment);

            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);

            await DoNavigateAction(GetCurrentPage(), nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(null, nextPage, useModalNavigation, animated);
            });
        }

        async Task ProcessNavigationForContentPage(Page currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextPage = CreatePageFromSegment(nextSegment);

            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);

            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated);
            });
        }

        async Task ProcessNavigationForNavigationPage(NavigationPage currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
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

            var currentNavRoot = currentPage.Navigation.NavigationStack[0];
            var nextPageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
            if (currentNavRoot.GetType() == nextPageType)
            {
                if (currentPage.Navigation.NavigationStack.Count > 1)
                    await currentPage.Navigation.PopToRootAsync(false);

                await ProcessNavigation(currentNavRoot, segments, parameters, false, animated);
                await DoNavigateAction(currentNavRoot, nextSegment, currentNavRoot, parameters);
                return;
            }
            else
            {
                await currentPage.Navigation.PopToRootAsync(false);
                var newRoot = CreatePageFromSegment(nextSegment);
                await ProcessNavigation(newRoot, segments, parameters, false, animated);

                await DoNavigateAction(currentNavRoot, nextSegment, newRoot, parameters, async () =>
                {
                    var push = DoPush(currentPage, newRoot, false, animated);
                    currentPage.Navigation.RemovePage(currentNavRoot);
                    await push;
                });
                return;
            }
        }

        async Task ProcessNavigationForTabbedPage(TabbedPage currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
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

        async Task ProcessNavigationForCarouselPage(CarouselPage currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
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

        async Task ProcessNavigationForMasterDetailPage(MasterDetailPage currentPage, string nextSegment, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
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
                });
                return;
            }
        }

        bool GetMasterDetailPageIsPresented(MasterDetailPage page)
        {
            var iMasterDetailPage = page as IMasterDetailPageOptions;
            if (iMasterDetailPage != null)
                return iMasterDetailPage.IsPresentedAfterNavigation;

            var iMasterDetailPageBindingContext = page.BindingContext as IMasterDetailPageOptions;
            if (iMasterDetailPageBindingContext != null)
                return iMasterDetailPageBindingContext.IsPresentedAfterNavigation;

            return false;
        }

        static async Task DoNavigateAction(Page fromPage, string toSegment, Page toPage, NavigationParameters parameters, Func<Task> navigationAction = null, Action onNavigationActionCompleted = null)
        {
            var segmentPrameters = GetSegmentParameters(toSegment, parameters);

            var canNavigate = await CanNavigateAsync(fromPage, segmentPrameters);
            if (!canNavigate)
                return;

            OnNavigatedFrom(fromPage, segmentPrameters);

            if (navigationAction != null)
                await navigationAction();

            onNavigationActionCompleted?.Invoke();

            OnNavigatedTo(toPage, segmentPrameters);
        }

        protected abstract Page CreatePage(string segmentName);

        Page CreatePageFromSegment(string segment)
        {
            try
            {
                var segmentName = UriParsingHelper.GetSegmentName(segment);
                var page = CreatePage(segmentName);
                if (page == null)
                    throw new NullReferenceException(string.Format("{0} could not be created. Please make sure you have registered {0} for navigation.", segmentName));

                return page;
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), Category.Exception, Priority.High);
                throw;
            }
        }

        static bool HasNavigationPageParent(Page page)
        {
            return page?.Parent != null && page?.Parent is NavigationPage;
        }

        static bool UseModalNavigation(Page currentPage, bool? useModalNavigationDefault)
        {
            bool useModalNavigation = true;

            if (useModalNavigationDefault.HasValue)
                useModalNavigation = useModalNavigationDefault.Value;
            else
                useModalNavigation = !HasNavigationPageParent(currentPage);

            return useModalNavigation;
        }

        Page GetOnNavigatedToTarget(Page page, bool useModalNavigation)
        {
            Page target = null;

            if (useModalNavigation)
            {
                var previousPage = GetPreviousPage(page, page.Navigation.ModalStack);

                //MainPage is not included in the navigation stack, so if we can't find the previous page above
                //let's assume they are going back to the MainPage
                target = GetOnNavigatedToTargetFromChild(previousPage ?? _applicationProvider.MainPage);
            }
            else
            {
                target = GetPreviousPage(page, page.Navigation.NavigationStack);
                if (target == null)
                    target = GetOnNavigatedToTarget(page, true);
            }

            return target;
        }

        static Page GetOnNavigatedToTargetFromChild(Page target)
        {
            Page child = null;

            if (target is MasterDetailPage)
                child = ((MasterDetailPage)target).Detail;
            else if (target is TabbedPage)
                child = ((TabbedPage)target).CurrentPage;
            else if (target is CarouselPage)
                child = ((CarouselPage)target).CurrentPage;
            else if (target is NavigationPage)
                child = target.Navigation.NavigationStack.Last();

            if (child != null)
                target = GetOnNavigatedToTargetFromChild(child);

            return target;
        }

        static Page GetPreviousPage(Page currentPage, IReadOnlyList<Page> navStack)
        {
            Page previousPage = null;

            int currentPageIndex = GetCurrentPageIndex(currentPage, navStack);
            int previousPageIndex = currentPageIndex - 1;
            if (navStack.Count >= 0 && previousPageIndex >= 0)
                previousPage = navStack[previousPageIndex];

            return previousPage;
        }

        static int GetCurrentPageIndex(Page currentPage, IReadOnlyList<Page> navStack)
        {
            int stackCount = navStack.Count;
            for (int x = 0; x < stackCount; x++)
            {
                var view = navStack[x];
                if (view == currentPage)
                    return x;
            }

            return stackCount - 1;
        }

        async Task DoPush(Page currentPage, Page page, bool? useModalNavigation, bool animated)
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

        static Task<Page> DoPop(INavigation navigation, bool useModalNavigation, bool animated)
        {
            if (useModalNavigation)
                return navigation.PopModalAsync(animated);
            else
                return navigation.PopAsync(animated);
        }

        static Task<bool> CanNavigateAsync(object page, NavigationParameters parameters)
        {
            var confirmNavigationItem = page as IConfirmNavigationAsync;
            if (confirmNavigationItem != null)
                return confirmNavigationItem.CanNavigateAsync(parameters);

            var bindableObject = page as BindableObject;
            if (bindableObject != null)
            {
                var confirmNavigationBindingContext = bindableObject.BindingContext as IConfirmNavigationAsync;
                if (confirmNavigationBindingContext != null)
                    return confirmNavigationBindingContext.CanNavigateAsync(parameters);
            }

            return Task.FromResult(CanNavigate(page, parameters));
        }

        static bool CanNavigate(object page, NavigationParameters parameters)
        {
            var confirmNavigationItem = page as IConfirmNavigation;
            if (confirmNavigationItem != null)
                return confirmNavigationItem.CanNavigate(parameters);

            var bindableObject = page as BindableObject;
            if (bindableObject != null)
            {
                var confirmNavigationBindingContext = bindableObject.BindingContext as IConfirmNavigation;
                if (confirmNavigationBindingContext != null)
                    return confirmNavigationBindingContext.CanNavigate(parameters);
            }

            return true;
        }

        static void OnNavigatedFrom(object page, NavigationParameters parameters)
        {
            if (page != null)
                InvokeOnNavigationAwareElement(page, v => v.OnNavigatedFrom(parameters));
        }

        static void OnNavigatedTo(object page, NavigationParameters parameters)
        {
            if (page != null)
                InvokeOnNavigationAwareElement(page, v => v.OnNavigatedTo(parameters));
        }

        static void InvokeOnNavigationAwareElement(object item, Action<INavigationAware> invocation)
        {
            var navigationAwareItem = item as INavigationAware;
            if (navigationAwareItem != null)
                invocation(navigationAwareItem);

            var bindableObject = item as BindableObject;
            if (bindableObject != null)
            {
                var navigationAwareDataContext = bindableObject.BindingContext as INavigationAware;
                if (navigationAwareDataContext != null)
                    invocation(navigationAwareDataContext);
            }
        }

        static NavigationParameters GetSegmentParameters(string uriSegment, NavigationParameters parameters)
        {
            var navParameters = UriParsingHelper.GetSegmentParameters(uriSegment);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> navigationParameter in parameters)
                {
                    navParameters.Add(navigationParameter.Key, navigationParameter.Value);
                }
            }

            return navParameters;
        }

        private Page GetCurrentPage()
        {
            return _page != null ? _page : _applicationProvider.MainPage;
        }
    }
}
