using Prism.Navigation;
using System;
using System.Linq;
using Xamarin.Forms;

namespace Prism.Common
{
    public static class PageUtilities
    {
        public static void InvokeViewAndViewModelAction<T>(object view, Action<T> action) where T : class
        {
            T viewAsT = view as T;
            if (viewAsT != null)
                action(viewAsT);

            var element = view as BindableObject;
            if (element != null)
            {
                var viewModelAsT = element.BindingContext as T;
                if (viewModelAsT != null)
                {
                    action(viewModelAsT);
                }
            }
        }

        public static void DestroyPage(Page page)
        {
            try
            {
                DestroyChildren(page);

                InvokeViewAndViewModelAction<IDestructible>(page, v => v.Destroy());

                page.Behaviors?.Clear();
                page.BindingContext = null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot destroy {page}: {ex.Message}");
            }
        }

        private static void DestroyChildren(Page page)
        {
            if (page is MasterDetailPage)
            {
                DestroyPage(((MasterDetailPage)page).Detail);
            }
            else if (page is TabbedPage)
            {
                var tabbedPage = (TabbedPage)page;
                foreach (var item in tabbedPage.Children)
                {
                    DestroyPage(item);
                }
            }
            else if (page is CarouselPage)
            {
                var carouselPage = (CarouselPage)page;
                foreach (var item in carouselPage.Children)
                {
                    DestroyPage(item);
                }
            }
            else if (page is NavigationPage)
            {
                var navigationPage = (NavigationPage)page;
                foreach (var item in navigationPage.Navigation.NavigationStack)
                {
                    DestroyPage(item);
                }
            }
        }

        public static void OnNavigatedFrom(object page, NavigationParameters parameters)
        {
            if (page != null)
                InvokeViewAndViewModelAction<INavigationAware>(page, v => v.OnNavigatedFrom(parameters));
        }

        public static void OnNavigatingTo(object page, NavigationParameters parameters)
        {
            if (page != null)
                InvokeViewAndViewModelAction<INavigationAware>(page, v => v.OnNavigatingTo(parameters));
        }

        public static void OnNavigatedTo(object page, NavigationParameters parameters)
        {
            if (page != null)
                InvokeViewAndViewModelAction<INavigationAware>(page, v => v.OnNavigatedTo(parameters));
        }

        public static Page GetOnNavigatedToTarget(Page page, Page mainPage, bool useModalNavigation)
        {
            Page target = null;

            if (useModalNavigation)
            {
                var previousPage = GetPreviousPage(page, page.Navigation.ModalStack);

                //MainPage is not included in the navigation stack, so if we can't find the previous page above
                //let's assume they are going back to the MainPage
                target = GetOnNavigatedToTargetFromChild(previousPage ?? mainPage);
            }
            else
            {
                target = GetPreviousPage(page, page.Navigation.NavigationStack);
                if (target == null)
                    target = GetOnNavigatedToTarget(page, mainPage, true);
            }

            return target;
        }

        public static Page GetOnNavigatedToTargetFromChild(Page target)
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

        public static Page GetPreviousPage(Page currentPage, System.Collections.Generic.IReadOnlyList<Page> navStack)
        {
            Page previousPage = null;

            int currentPageIndex = GetCurrentPageIndex(currentPage, navStack);
            int previousPageIndex = currentPageIndex - 1;
            if (navStack.Count >= 0 && previousPageIndex >= 0)
                previousPage = navStack[previousPageIndex];

            return previousPage;
        }

        public static int GetCurrentPageIndex(Page currentPage, System.Collections.Generic.IReadOnlyList<Page> navStack)
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
    }
}
