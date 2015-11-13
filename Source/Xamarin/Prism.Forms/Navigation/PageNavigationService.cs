using System;
using System.Diagnostics;
using Xamarin.Forms;
using Prism.Common;
using System.Collections.Generic;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides page based navigation for ViewModels.
    /// </summary>
    public abstract class PageNavigationService : INavigationService, IPageAware
    {
        private Page _page;
        Page IPageAware.Page
        {
            get { return _page; }
            set { _page = value; }
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        public void GoBack(bool useModalNavigation = true, bool animated = true)
        {
            var page = GetRootPage();
            DoPop(page.Navigation, useModalNavigation, animated);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type which will be used to identify the name of the navigation target.</typeparam>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        public void Navigate<T>(NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            Navigate(typeof(T).FullName, parameters, useModalNavigation, animated);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        public void Navigate(string name, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            Navigate(new Uri(name, UriKind.RelativeOrAbsolute), parameters, useModalNavigation, animated);
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
        public void Navigate(Uri uri, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            var navigationSegments = UriParsingHelper.GetUriSegments(uri);

            var isDeepLink = navigationSegments.Count > 1;
            ProcessNavigationSegments(GetRootPage(), navigationSegments, parameters, useModalNavigation, isDeepLink ? false : animated);
        }

        void ProcessNavigationSegments(Page currentPage, Queue<string> segments, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            if (segments.Count == 0)
                return;

            var segment = segments.Dequeue();
            var segmentName = UriParsingHelper.GetSegmentName(segment);

            var targetPage = CreatePage(segmentName);
            if (targetPage != null)
            {
                bool useModalForDoNav = UseModalNavigation(currentPage, UriParsingHelper.GetSegmentName(segment), useModalNavigation);

                ProcessNavigationForPages(currentPage, segment, targetPage, segments, parameters, useModalForDoNav, animated);
            }
            else
            {
                //TODO: log this using the logger
                Debug.WriteLine("Navigation ERROR: {0} not found. Make sure you have registered {0} for navigation.", segmentName);

                //TODO: since an error occured, should we stop processing, or continue?
                //ProcessNavigationSegments(currentPage, segments, parameters, useModalNavigation, animated);
            }
        }

        void ProcessNavigationForPages(Page currentPage, string targetSegment, Page targetPage, Queue<string> segments, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            if (targetPage is ContentPage)
            {
                ProcessNavigationForContentPage(currentPage, targetSegment, (ContentPage)targetPage, segments, parameters, useModalNavigation, animated);
            }
            else if (targetPage is NavigationPage)
            {
                ProcessNavigationForNavigationPage(currentPage, targetSegment, (NavigationPage)targetPage, segments, parameters, useModalNavigation, animated);
            }
            else if (targetPage is TabbedPage)
            {
                ProcessNavigationForTabbedPage(currentPage, targetSegment, (TabbedPage)targetPage, segments, parameters, useModalNavigation, animated);
            }
            //else if (targetPage is CarouselPage)
            //{

            //}
            else if (targetPage is MasterDetailPage)
            {
                ProcessNavigationForMasterDetailPage(currentPage, targetSegment, (MasterDetailPage)targetPage, segments, parameters, useModalNavigation, animated);
            }
        }

        void ProcessNavigationForContentPage(Page currentPage, string targetSegment, ContentPage targetPage, Queue<string> segments, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            ProcessNavigationSegments(targetPage, segments, parameters, useModalNavigation, animated);
            DoNavigate(currentPage, targetSegment, targetPage, parameters, useModalNavigation, animated);
        }

        void ProcessNavigationForNavigationPage(Page currentPage, string targetSegment, NavigationPage targetPage, Queue<string> segments, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            if (targetPage.Navigation.NavigationStack.Count == 0)
            {
                ProcessNavigationSegments(targetPage, segments, parameters, false, animated);
                DoNavigate(currentPage, targetSegment, targetPage, parameters, useModalNavigation, animated);
                return;
            }

            if (segments.Count > 0) // we have a next page
            {
                var currentNavRoot = targetPage.Navigation.NavigationStack[0];
                var nextPageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(segments.Peek()));
                if (currentNavRoot.GetType() == nextPageType)
                {
                    if (targetPage.Navigation.NavigationStack.Count > 1)
                        targetPage.Navigation.PopToRootAsync(false);

                    segments.Dequeue();
                    ProcessNavigationSegments(currentNavRoot, segments, parameters, false, animated);
                    DoNavigate(currentPage, targetSegment, targetPage, parameters, useModalNavigation, animated);
                    return;
                }
                else
                {
                    targetPage.Navigation.PopToRootAsync(false);
                    ProcessNavigationSegments(targetPage, segments, parameters, false, animated);
                    DoNavigate(currentPage, targetSegment, targetPage, parameters, useModalNavigation, animated);
                    targetPage.Navigation.RemovePage(currentNavRoot);
                    return;
                }
            }

            ProcessNavigationSegments(targetPage, segments, parameters, useModalNavigation, animated);
            DoNavigate(currentPage, targetSegment, targetPage, parameters, useModalNavigation, animated);
        }

        void ProcessNavigationForTabbedPage(Page currentPage, string targetSegment, TabbedPage targetPage, Queue<string> segments, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            if (segments.Count > 0) // we have a next page
            {
                var nextSegmentType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(segments.Peek()));
                foreach (var child in targetPage.Children)
                {
                    if (child.GetType() != nextSegmentType)
                        continue;

                    segments.Dequeue();
                    ProcessNavigationSegments(child, segments, parameters, useModalNavigation, animated);
                    targetPage.CurrentPage = child;
                    DoNavigate(currentPage, targetSegment, targetPage, parameters, useModalNavigation, animated);
                    return;
                }
            }

            ProcessNavigationSegments(targetPage, segments, parameters, useModalNavigation, animated);
            DoNavigate(currentPage, targetSegment, targetPage, parameters, useModalNavigation, animated);
        }

        void ProcessNavigationForMasterDetailPage(Page currentPage, string targetSegment, MasterDetailPage targetPage, Queue<string> segments, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            var detail = targetPage.Detail;
            if (detail == null)
            {
                var newDetail = CreatePage(segments.Dequeue());
                ProcessNavigationSegments(newDetail, segments, parameters, newDetail is NavigationPage ? false : true, animated);
                targetPage.Detail = newDetail;
                DoNavigate(currentPage, targetSegment, targetPage, parameters, useModalNavigation, animated);
                return;
            }

            if (segments.Count > 0) // we have a next page
            {
                var nextSegmentType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(segments.Peek()));
                if (detail.GetType() == nextSegmentType)
                {
                    segments.Dequeue();
                    ProcessNavigationSegments(detail, segments, parameters, useModalNavigation, animated);
                    DoNavigate(currentPage, targetSegment, targetPage, parameters, useModalNavigation, animated);
                    return;
                }
            }

            ProcessNavigationSegments(targetPage, segments, parameters, useModalNavigation, animated);
            DoNavigate(currentPage, targetSegment, targetPage, parameters, useModalNavigation, animated);
        }

        protected abstract Page CreatePage(string name);

        static bool HasNavigationPageParent(Page page)
        {
            //TODO:  may need to traverse the navigation stack to look for a NavigationPage
            //good chance there is another page type, like a TabbedPage, within a NavigationPage
            return page.Parent != null && page.Parent is NavigationPage;
        }

        static bool UseModalNavigation(Page currentPage, string name, bool? useModalNavigationDefault)
        {
            bool useModalNavigation = true;

            if (useModalNavigationDefault.HasValue)
                useModalNavigation = useModalNavigationDefault.Value;
            else
                useModalNavigation = !HasNavigationPageParent(currentPage);

            //TODO: think about using an interface instead to give the developer a hook to perform conditional logic to return the proper result
            var navParams = PageNavigationRegistry.GetPageNavigationOptions(name);
            if (navParams != null)
                useModalNavigation = navParams.UseModalNavigation;

            return useModalNavigation;
        }

        static bool AnimateNavigation(string name, bool animateDefault)
        {
            bool animate = animateDefault;

            var navParams = PageNavigationRegistry.GetPageNavigationOptions(name);
            if (navParams != null)
                animate = navParams.Animated;

            return animate;
        }

        async static void DoPush(INavigation navigation, Page page, bool useModalNavigation, bool animated)
        {
            if (useModalNavigation)
                await navigation.PushModalAsync(page, animated);
            else
                await navigation.PushAsync(page, animated);
        }

        async static void DoPop(INavigation navigation, bool useModalNavigation, bool animated)
        {
            if (useModalNavigation)
                await navigation.PopModalAsync(animated);
            else
                await navigation.PopAsync(animated);
        }

        static void DoNavigate(Page currentPage, string segment, Page targetPage, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            var segmentPrameters = GetSegmentParameters(segment, parameters);

            if (!CanNavigate(currentPage, segmentPrameters))
                return;

            OnNavigatedFrom(currentPage, segmentPrameters);

            DoPush(currentPage.Navigation, targetPage, useModalNavigation, animated);

            OnNavigatedTo(targetPage, segmentPrameters);
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
            var currentPage = page as Page;
            if (currentPage != null)
                InvokeOnNavigationAwareElement(currentPage, v => v.OnNavigatedFrom(parameters));
        }

        static void OnNavigatedTo(object page, NavigationParameters parameters)
        {
            var currentPage = page as Page;

            if (currentPage is NavigationPage)
            {
                var navpage = (NavigationPage)page;
                InvokeOnNavigationAwareElement(navpage.CurrentPage, v => v.OnNavigatedTo(parameters));
            }
            else if (currentPage is MasterDetailPage)
            {
                var mdPage = (MasterDetailPage)page;
                InvokeOnNavigationAwareElement(mdPage.Detail, v => v.OnNavigatedTo(parameters));
            }

            if (currentPage != null)
                InvokeOnNavigationAwareElement(currentPage, v => v.OnNavigatedTo(parameters));
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

            if (navParameters.Count == 0)
                return null;

            return navParameters;
        }

        Page GetRootPage()
        {
            return _page != null ? _page : Application.Current.MainPage;
        }
    }
}
