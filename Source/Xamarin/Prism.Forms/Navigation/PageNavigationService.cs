using System;
using System.Diagnostics;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;
using Prism.Common;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides page based navigation for ViewModels.
    /// </summary>
    public class PageNavigationService : INavigationService, IPageAware
    {
        static Dictionary<Type, INavigationPageProvider> _navigationProviderCache = new Dictionary<Type, INavigationPageProvider>();
        static Dictionary<Type, PageNavigationParametersAttribute> _navigationServiceParametersAttributeCache = new Dictionary<Type, PageNavigationParametersAttribute>();

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
            var navigation = GetPageNavigation();
            DoPop(navigation, useModalNavigation, animated);
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
            var deepLinkSegments = UriParsingHelper.GetSegmentQue(uri);

            if (deepLinkSegments.Count > 1)
            {
                var currentNavigation = GetPageNavigation();
                var rootPage = currentNavigation.ModalStack.Last();

                HandleDeepLinkNavigation(rootPage, deepLinkSegments);
            }
            else
            {
                //TODO: may not need this
                var name = UriParsingHelper.GetAbsolutePath(uri).TrimStart('/');
                var navParameters = GetParametersForUriNavigation(uri, parameters);
                Navigate(name, navParameters, useModalNavigation, animated);
            }
        }

        private static void HandleDeepLinkNavigation(Page currentPage, Queue<string> segments)
        {
            if (segments.Count == 0)
                return;

            var nextSegment = segments.Dequeue();
            var nextSegmentName = UriParsingHelper.GetSegmentName(nextSegment);

            var targetPage = ServiceLocator.Current.GetInstance<object>(nextSegmentName) as Page;
            if (targetPage != null)
            {
                Page navigationPageFromProvider = GetPageFromProvider(currentPage, targetPage);

                if (navigationPageFromProvider is NavigationPage)
                {
                    HandleNavigationForNavigationPage((NavigationPage)navigationPageFromProvider, segments);
                }
                else if (navigationPageFromProvider is TabbedPage || navigationPageFromProvider is CarouselPage)
                {
                    HandleNavigationForMultiPage((TabbedPage)navigationPageFromProvider, segments);
                }
                else if (navigationPageFromProvider is MasterDetailPage)
                {
                    HandleNavigationForMasterDetailPage((NavigationPage)navigationPageFromProvider, segments);
                }
                else
                {
                    HandleDeepLinkNavigation(navigationPageFromProvider, segments);
                }


                //Should we call OnNavigatedFrom during a deep link?
                //OnNavigatedFrom(targetView, nextSegmentPrameters);

                bool useModalNavigation = true;

                var navParams = GetNavigationServiceParametersAttribute(targetPage);             
                if (navParams != null)
                    useModalNavigation = navParams.UseModalNavigation;

                var nextSegmentPrameters = UriParsingHelper.GetSegmentParameters(nextSegment);

                DoPush(currentPage.Navigation, navigationPageFromProvider, useModalNavigation, false);

                OnNavigatedTo(navigationPageFromProvider, nextSegmentPrameters);
            }
            else
            {
                HandleDeepLinkNavigation(currentPage, segments);
            }
        }

        private static void HandleNavigationForMasterDetailPage(NavigationPage targetPage, Queue<string> segments)
        {
            //TODO:  implement
            HandleDeepLinkNavigation(targetPage, segments);
        }

        private static void HandleNavigationForMultiPage(TabbedPage targetPage, Queue<string> segments)
        {
            //var nextSegmentType = PageNavigationRegistry.PageRegistrationCache[segments.Peek()];
            var nextSegmentType = PageNavigationRegistry.GetPageType(segments.Peek());

            foreach (var child in targetPage.Children)
            {
                if (child.GetType() != nextSegmentType)
                    continue;

                segments.Dequeue();
                HandleDeepLinkNavigation(child, segments);
                targetPage.CurrentPage = child;
                return;
            }

            HandleDeepLinkNavigation(targetPage, segments);
        }

        private static void HandleNavigationForContentPage(ContentPage targetPage, Queue<string> segments)
        {
            HandleDeepLinkNavigation(targetPage, segments);
        }

        private static void HandleNavigationForNavigationPage(NavigationPage targetPage, Queue<string> segments)
        {
            try
            {
                if (targetPage.Navigation.NavigationStack.Count == 0)
                {
                    HandleDeepLinkNavigation(targetPage, segments);
                }
                else
                {
                    HandleDeepLinkNavigation(targetPage, segments);

                    // if the target page was wrapped using a NavigationPageProvider, we don't want to remove the root
                    // otherwise we always remove the root
                    var currentNavRoot = targetPage.Navigation.NavigationStack[0];
                    var currentNavRootType = currentNavRoot.GetType();
                    if (!_navigationProviderCache.ContainsKey(currentNavRootType) ||
                        (_navigationProviderCache.ContainsKey(currentNavRootType) && _navigationProviderCache[currentNavRootType] == null))
                    {
                        targetPage.Navigation.RemovePage(currentNavRoot);
                    }
                }
            }
            catch (Exception ex)
            {

            }
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
            var targetView = ServiceLocator.Current.GetInstance<object>(name) as Page;
            if (targetView != null)
            {
                var navigation = GetPageNavigation();

                if (!CanNavigate(_page, parameters))
                    return;

                Page navigationPageFromProvider = GetPageFromProvider(_page, targetView);

                OnNavigatedFrom(_page, parameters);

                DoPush(navigation, navigationPageFromProvider, useModalNavigation, animated);

                OnNavigatedTo(navigationPageFromProvider, parameters);
            }
            else
                Debug.WriteLine("Navigation ERROR: {0} not found. Make sure you have registered {0} for navigation.", name);
        }

        private async static void DoPush(INavigation navigation, Page view, bool useModalNavigation, bool animated)
        {
            if (useModalNavigation)
                await navigation.PushModalAsync(view, animated);
            else
                await navigation.PushAsync(view, animated);
        }

        private async static void DoPop(INavigation navigation, bool useModalNavigation, bool animated)
        {
            if (useModalNavigation)
                await navigation.PopModalAsync(animated);
            else
                await navigation.PopAsync(animated);
        }

        private INavigation GetPageNavigation()
        {
            return _page != null ? _page.Navigation : Application.Current.MainPage.Navigation;
        }

        private static bool CanNavigate(object item, NavigationParameters parameters)
        {
            var confirmNavigationItem = item as IConfirmNavigation;
            if (confirmNavigationItem != null)
                return confirmNavigationItem.CanNavigate(parameters);

            var bindableObject = item as BindableObject;
            if (bindableObject != null)
            {
                var confirmNavigationBindingContext = bindableObject.BindingContext as IConfirmNavigation;
                if (confirmNavigationBindingContext != null)
                    return confirmNavigationBindingContext.CanNavigate(parameters);
            }

            return true;
        }

        private static void OnNavigatedFrom(object page, NavigationParameters parameters)
        {
            var currentPage = page as Page;
            if (currentPage != null)
                InvokeOnNavigationAwareElement(currentPage, v => v.OnNavigatedFrom(parameters));
        }

        private static void OnNavigatedTo(object page, NavigationParameters parameters)
        {
            Page currentPage = null;

            if (page is NavigationPage)
            {
                var navpage = (NavigationPage)page;
                currentPage = navpage.CurrentPage;
            }
            else
            {
                currentPage = page as Page;
            }

            if (currentPage != null)
                InvokeOnNavigationAwareElement(currentPage, v => v.OnNavigatedTo(parameters));
        }

        private static void InvokeOnNavigationAwareElement(object item, Action<INavigationAware> invocation)
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

        private static Page GetPageFromProvider(Page sourceView, Page targetView)
        {
            INavigationPageProvider provider = null;
            Type viewType = targetView.GetType();

            if (_navigationProviderCache.ContainsKey(viewType))
            {
                provider = _navigationProviderCache[viewType];
            }
            else
            {
                var navigationPageProvider = viewType.GetTypeInfo().GetCustomAttribute<NavigationPageProviderAttribute>(true);
                if (navigationPageProvider != null)
                {
                    provider = ServiceLocator.Current.GetInstance(navigationPageProvider.Type) as INavigationPageProvider;
                    if (provider == null)
                        throw new InvalidCastException("Could not create the navigation page provider.  Please make sure the navigation page provider implements the INavigationPageProvider interface.");
                }
            }

            if (!_navigationProviderCache.ContainsKey(viewType))
                _navigationProviderCache.Add(viewType, provider);

            if (provider != null)
                return provider.CreatePageForNavigation(sourceView, targetView);

            // if no provider found return the targetView
            return targetView;
        }

        private static PageNavigationParametersAttribute GetNavigationServiceParametersAttribute(Page page)
        {
            PageNavigationParametersAttribute navParams = null;
            Type pageType = page.GetType();

            if (_navigationServiceParametersAttributeCache.ContainsKey(pageType))
                navParams = _navigationServiceParametersAttributeCache[pageType];
            else
                navParams = pageType.GetTypeInfo().GetCustomAttribute<PageNavigationParametersAttribute>();

            if (!_navigationServiceParametersAttributeCache.ContainsKey(pageType))
                _navigationServiceParametersAttributeCache.Add(pageType, navParams);

            return navParams;
        }

        NavigationParameters GetParametersForUriNavigation(Uri uri, NavigationParameters parameters)
        {
            var navParameters = UriParsingHelper.GetParametersFromUri(uri);

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
    }
}
