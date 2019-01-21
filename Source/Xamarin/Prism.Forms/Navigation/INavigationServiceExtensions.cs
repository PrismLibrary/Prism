using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Navigation
{
    public static class INavigationServiceExtensions
    {
        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        public static Task<INavigationResult> GoBackAsync(this INavigationService navigationService, INavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            return ((IPlatformNavigationService)navigationService).GoBackAsync(parameters, useModalNavigation, animated);
        }

        /// <summary>
        /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
        /// </summary>
        /// <param name="navigationService">The INavigatinService instance</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Only works when called from a View within a NavigationPage</remarks>
        public static Task<INavigationResult> GoBackToRootAsync(this INavigationService navigationService, INavigationParameters parameters = null)
        {
            return ((IPlatformNavigationService)navigationService).GoBackToRootAsync(parameters);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PushModalAsync, if <c>false</c> uses PushAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, string name, INavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            return ((IPlatformNavigationService)navigationService).NavigateAsync(name, parameters, useModalNavigation, animated);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// NavigateAsync(new Uri("MainPage?id=3&name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, Uri uri, INavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            return ((IPlatformNavigationService)navigationService).NavigateAsync(uri, parameters, useModalNavigation, animated);
        }

        /// <summary>
        /// Gets an absolute path of the current page as it relates to it's position in the navigation stack.
        /// </summary>
        /// <returns>The absolute path of the current Page</returns>
        public static string GetNavigationUriPath(this INavigationService navigationService)
        {
            var currentpage = ((IPageAware)navigationService).Page;

            Stack<string> stack = new Stack<string>();
            currentpage = ProcessCurrentPageNavigationPath(currentpage, stack);
            ProcessNavigationPath(currentpage, stack);

            StringBuilder sb = new StringBuilder();
            while (stack.Count > 0)
            {
                sb.Append($"/{stack.Pop()}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        public static Task<INavigationResult> GoBackAsync(this INavigationService navigationService, params (string Key, object Value)[] parameters)
        {
            return navigationService.GoBackAsync(GetNavigationParameters(parameters));
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// NavigateAsync("MainPage?id=3&name=dan", ("person", person), ("foo", bar));
        /// </example>
        public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, string name, params (string Key, object Value)[] parameters)
        {
            return navigationService.NavigateAsync(name, GetNavigationParameters(parameters));
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// NavigateAsync(new Uri("MainPage?id=3&name=dan", UriKind.RelativeSource), ("person", person), ("foo", bar));
        /// </example>
        public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, Uri uri, params (string Key, object Value)[] parameters)
        {
            return navigationService.NavigateAsync(uri, GetNavigationParameters(parameters));
        }

        private static INavigationParameters GetNavigationParameters((string Key, object Value)[] parameters)
        {
            var navParams = new NavigationParameters();
            foreach(var (Key, Value) in parameters)
            {
                navParams.Add(Key, Value);
            }
            return navParams;
        }

        private static void ProcessNavigationPath(Page page, Stack<string> stack)
        {
            if (page.Parent is Page parent)
            {
                if (parent is NavigationPage)
                {
                    var index = PageUtilities.GetCurrentPageIndex(page, page.Navigation.NavigationStack);
                    if (index > 0)
                    {
                        int previousPageIndex = index - 1;
                        for (int x = previousPageIndex; x >= 0; x--)
                        {
                            AddSegmentToStack(page.Navigation.NavigationStack[x], stack);
                        }
                    }

                    AddSegmentToStack(parent, stack);
                }
                else if (parent is MasterDetailPage)
                {
                    AddSegmentToStack(parent, stack);
                }

                ProcessNavigationPath(parent, stack);
            }
            else
            {
                ProcessModalNavigation(page, stack);
            }
        }

        private static void ProcessModalNavigation(Page page, Stack<string> stack)
        {
            var index = PageUtilities.GetCurrentPageIndex(page, page.Navigation.ModalStack);
            int previousPageIndex = index - 1;
            for (int x = previousPageIndex; x >= 0; x--)
            {
                var childPage = page.Navigation.ModalStack[x];
                if (childPage is NavigationPage)
                {
                    AddUseModalNavigationParameter(stack);
                    ProcessModalNavigationPagePath((NavigationPage)childPage, stack);
                }
                else if (childPage is MasterDetailPage)
                {
                    ProcessModalMasterDetailPagePath((MasterDetailPage)childPage, stack);
                }
                else
                {
                    AddSegmentToStack(childPage, stack);
                }
            }

            ProcessMainPagePath(Application.Current?.MainPage, page, stack);
        }

        private static void ProcessMainPagePath(Page mainPage, Page previousPage, Stack<string> stack)
        {
            if (mainPage == null)
                return;

            if (previousPage == mainPage)
                return;

            if (mainPage is NavigationPage)
            {
                AddUseModalNavigationParameter(stack);
                ProcessModalNavigationPagePath((NavigationPage)mainPage, stack);
            }
            else if (mainPage is MasterDetailPage)
            { 
                var detail = ((MasterDetailPage)mainPage).Detail;
                if (detail is NavigationPage)
                {
                    AddUseModalNavigationParameter(stack);
                    ProcessModalNavigationPagePath((NavigationPage)detail, stack);
                }
                else
                {
                    AddSegmentToStack(detail, stack);
                }

                AddSegmentToStack(mainPage, stack);
            }  
            else
            {
                AddSegmentToStack(mainPage, stack);
            }
        }

        private static void ProcessModalNavigationPagePath(NavigationPage page, Stack<string> stack)
        {
            var navStack = page.Navigation.NavigationStack.Reverse();
            foreach (var child in navStack)
            {
                AddSegmentToStack(child, stack);
            }

            AddSegmentToStack(page, stack);
        }

        private static void ProcessModalMasterDetailPagePath(MasterDetailPage page, Stack<string> stack)
        {
            if (page.Detail is NavigationPage)
            {
                AddUseModalNavigationParameter(stack);
                ProcessModalNavigationPagePath((NavigationPage)page.Detail, stack);
            }
            else
            {
                AddSegmentToStack(page.Detail, stack);
            }

            AddSegmentToStack(page, stack);
        }

        private static Page ProcessCurrentPageNavigationPath(Page page, Stack<string> stack)
        {
            var currentPageKeyInfo = PageNavigationRegistry.GetPageNavigationInfo(page.GetType());
            string currentSegment = $"{currentPageKeyInfo.Name}";

            if (page.Parent is Page parent)
            {
                var parentKeyInfo = PageNavigationRegistry.GetPageNavigationInfo(parent.GetType());

                if (parent is TabbedPage || parent is CarouselPage)
                {
                    //set the selected tab to the current page
                    currentSegment = $"{parentKeyInfo.Name}?{KnownNavigationParameters.SelectedTab}={currentPageKeyInfo.Name}";
                    page = parent;
                }
                else if (parent is MasterDetailPage)
                {
                    currentSegment = $"{parentKeyInfo.Name}/{currentPageKeyInfo.Name}";
                    page = parent;
                }
            }

            stack.Push(currentSegment);

            return page;
        }

        private static void AddSegmentToStack(Page page, Stack<string> stack)
        {
            if (page == null)
                return;

            var keyInfo = PageNavigationRegistry.GetPageNavigationInfo(page.GetType());
            if (keyInfo != null)
                stack.Push(keyInfo.Name);
        }

        private static void AddUseModalNavigationParameter(Stack<string> stack)
        {
            var lastPageName = stack.Pop();
            lastPageName = $"{lastPageName}?{KnownNavigationParameters.UseModalNavigation}=true";
            stack.Push(lastPageName);
        }
    }
}


