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
            var deepLinkSegments = UriParsingHelper.GetSegmentStack(uri);

            if (deepLinkSegments.Count > 1)
            {
                var currentNavigation = GetPageNavigation();
                var rootPage = currentNavigation.ModalStack.Last();

                HandleDeepLinkNavigation(rootPage, deepLinkSegments);
            }
            else
            {
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

            var targetView = ServiceLocator.Current.GetInstance<object>(nextSegmentName) as Page;
            if (targetView != null)
            {
                Page pageFromProvider = GetPageFromProvider(currentPage, targetView);

                HandleDeepLinkNavigation(pageFromProvider, segments);

                if (pageFromProvider is ContentPage)
                {
                    
                }
                if (pageFromProvider is NavigationPage)
                {
                    
                }
                if (pageFromProvider is TabbedPage)
                {
                    
                }
                if (pageFromProvider is CarouselPage)
                {
                    
                }
                if (pageFromProvider is MasterDetailPage)
                {
                    
                }

                bool useModalNavigation = GetDeepLinkNavigationMode(pageFromProvider) == NavigationMode.Modal;

                var nextSegmentPrameters = UriParsingHelper.GetSegmentParameters(nextSegment);

                //Should we call OnNavigatedFrom during a deep link?
                //OnNavigatedFrom(targetView, nextSegmentPrameters); 

                DoPush(currentPage.Navigation, pageFromProvider, useModalNavigation, false);

                OnNavigatedTo(targetView, nextSegmentPrameters);
            }
            else
            {
                HandleDeepLinkNavigation(currentPage, segments);
            }
        }

        private static NavigationMode GetDeepLinkNavigationMode(Page page)
        {
            var deepLinkNavigationMode = NavigationMode.Modal;

            var deepLinkInfo = page.GetType().GetTypeInfo().GetCustomAttribute<NavigationDeepLinkAttribute>();
            if (deepLinkInfo != null)
            {
                deepLinkNavigationMode = deepLinkInfo.NavigationMode;
            }

            return deepLinkNavigationMode;
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

                OnNavigatedTo(targetView, parameters);
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
            var currentPage = page as Page;
            if (currentPage != null)
                InvokeOnNavigationAwareElement(page, v => v.OnNavigatedTo(parameters));
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

        static Dictionary<Type, INavigationPageProvider> _navigationProviderCache = new Dictionary<Type, INavigationPageProvider>();

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
