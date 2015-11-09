using System;
using System.Diagnostics;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;
using Prism.Common;
using System.Collections.Generic;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides page based navigation for ViewModels.
    /// </summary>
    public class PageNavigationService : INavigationService, IPageAware
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
        public void Navigate<T>(NavigationParameters parameters = null, bool useModalNavigation = true, bool animated = true)
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
        public void Navigate(string name, NavigationParameters parameters = null, bool useModalNavigation = true, bool animated = true)
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
        public void Navigate(Uri uri, NavigationParameters parameters = null, bool useModalNavigation = true, bool animated = true)
        {
            var navigationSegments = UriParsingHelper.GetSegmentQue(uri);
            ProcessNavigationSegments(GetRootPage(), navigationSegments, parameters, useModalNavigation, navigationSegments.Count > 1 ? false : animated);
        }

        static void ProcessNavigationSegments(Page currentPage, Queue<string> segments, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            if (segments.Count == 0)
                return;

            var segment = segments.Dequeue();
            var segmentName = UriParsingHelper.GetSegmentName(segment);

            var targetPage = CreatePage(segmentName, currentPage);
            if (targetPage != null)
            {
                var segmentPrameters = GetSegmentParameters(segment, parameters);

                if (!CanNavigate(currentPage, parameters))
                    return;

                ProcessNavigationForPages(targetPage, segments, parameters, useModalNavigation, animated);

                DoNavigate(currentPage, targetPage, segmentPrameters, UseModalNavigation(segmentName, useModalNavigation), AnimateNavigation(segmentName, animated));
            }
            else
            {
                Debug.WriteLine("Navigation ERROR: {0} not found. Make sure you have registered {0} for navigation.", segmentName);

                //something went wrong and the page couldn't be created, so let's try and move on to the next segment in the uri
                ProcessNavigationSegments(currentPage, segments, parameters, useModalNavigation, animated);
            }
        }

        static void ProcessNavigationForPages(Page targetPage, Queue<string> segments, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            if (targetPage is NavigationPage)
            {
                ProcessNavigationForNavigationPage((NavigationPage)targetPage, segments, parameters, useModalNavigation, animated);
            }
            else if (targetPage is TabbedPage || targetPage is CarouselPage)
            {
                ProcessNavigationForMultiPage((TabbedPage)targetPage, segments, parameters, useModalNavigation, animated);
            }
            else if (targetPage is MasterDetailPage)
            {
                ProcessNavigationForMasterDetailPage((MasterDetailPage)targetPage, segments, parameters, useModalNavigation, animated);
            }
            else
            {
                ProcessNavigationSegments(targetPage, segments, parameters, useModalNavigation, animated);
            }
        }

        static void ProcessNavigationForNavigationPage(NavigationPage targetPage, Queue<string> segments, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            if (targetPage.Navigation.NavigationStack.Count == 0)
            {
                ProcessNavigationSegments(targetPage, segments, parameters, useModalNavigation, animated);
                return;
            }

            if (segments.Count > 0) // we have a next page
            {
                var currentNavRoot = targetPage.Navigation.NavigationStack[0];
                var nextPageType = PageNavigationRegistry.GetPageType(segments.Peek());
                if (currentNavRoot.GetType() == nextPageType)
                {
                    segments.Dequeue();
                    ProcessNavigationSegments(currentNavRoot, segments, parameters, useModalNavigation, animated);
                    return;
                }
            }

            ProcessNavigationSegments(targetPage, segments, parameters, useModalNavigation, animated);
        }

        static void ProcessNavigationForMultiPage(TabbedPage targetPage, Queue<string> segments, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            if (segments.Count > 0) // we have a next page
            {
                var nextSegmentType = PageNavigationRegistry.GetPageType(segments.Peek());

                foreach (var child in targetPage.Children)
                {
                    if (child.GetType() != nextSegmentType)
                        continue;

                    segments.Dequeue();
                    ProcessNavigationSegments(child, segments, parameters, useModalNavigation, animated);
                    targetPage.CurrentPage = child;
                    return;
                }
            }

            ProcessNavigationSegments(targetPage, segments, parameters, useModalNavigation, animated);
        }

        static void ProcessNavigationForMasterDetailPage(MasterDetailPage targetPage, Queue<string> segments, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            var detail = targetPage.Detail;
            if (detail == null)
            {
                var newDetail = CreatePage(segments.Dequeue(), targetPage);
                ProcessNavigationSegments(newDetail, segments, parameters, useModalNavigation, animated);
                targetPage.Detail = newDetail;
                return;
            }

            if (segments.Count > 0) // we have a next page
            {
                var nextSegmentType = PageNavigationRegistry.GetPageType(segments.Peek());
                if (detail.GetType() == nextSegmentType)
                {
                    segments.Dequeue();
                    ProcessNavigationSegments(detail, segments, parameters, useModalNavigation, animated);
                    return;
                }
            }

            ProcessNavigationSegments(targetPage, segments, parameters, useModalNavigation, animated);
        }

        static Page CreatePage(string name)
        {
            return ServiceLocator.Current.GetInstance<object>(name) as Page;
        }

        static Page CreatePage(string name, Page currentPage)
        {
            var targetPage = CreatePage(name);
            if (targetPage != null)
                targetPage = GetPageFromProvider(name, currentPage, targetPage);
            return targetPage;
        }

        static Page GetPageFromProvider(string name, Page sourceView, Page targetView)
        {
            Type viewType = targetView.GetType();

            IPageNavigationProvider provider = PageNavigationRegistry.GetPageNavigationProvider(name);            
            if (provider != null)
                return provider.CreatePageForNavigation(sourceView, targetView);

            return targetView;
        }

        static bool UseModalNavigation(string name, bool useModalNavigationDefault)
        {
            bool useModalNavigation = useModalNavigationDefault;

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

        static void DoNavigate(Page currentPage, Page targetPage, NavigationParameters parameters, bool useModalNavigation, bool animated)
        {
            OnNavigatedFrom(currentPage, parameters);

            DoPush(currentPage.Navigation, targetPage, useModalNavigation, animated);

            OnNavigatedTo(targetPage, parameters);
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
