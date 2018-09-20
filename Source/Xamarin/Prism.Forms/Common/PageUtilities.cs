using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Common
{
    public static class PageUtilities
    {
        public static void InvokeViewAndViewModelAction<T>(object view, Action<T> action) where T : class
        {
            if (view is T viewAsT)
                action(viewAsT);

            if (view is BindableObject element)
            {
                if (element.BindingContext is T viewModelAsT)
                {
                    action(viewModelAsT);
                }
            }

            if(view is Page page)
            {
                var partials = (List<BindableObject>)page.GetValue(ViewModelLocator.PartialViewsProperty) ?? new List<BindableObject>();
                foreach(var partial in partials)
                {
                    InvokeViewAndViewModelAction(partial, action);
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
                throw new Exception($"Cannot destroy {page}.", ex);
            }
        }

        private static void DestroyChildren(Page page)
        {
            if (page is MasterDetailPage)
            {
                DestroyPage(((MasterDetailPage)page).Master);
                DestroyPage(((MasterDetailPage)page).Detail);
            }
            else if (page is TabbedPage tabbedPage)
            {
                foreach (var item in tabbedPage.Children.Reverse())
                {
                    DestroyPage(item);
                }
            }
            else if (page is CarouselPage carouselPage)
            {
                foreach (var item in carouselPage.Children.Reverse())
                {
                    DestroyPage(item);
                }
            }
            else if (page is NavigationPage navigationPage)
            {
                foreach (var item in navigationPage.Navigation.NavigationStack.Reverse())
                {
                    DestroyPage(item);
                }
            }
        }

        public static void DestroyWithModalStack(Page page, IList<Page> modalStack)
        {
            foreach (var childPage in modalStack.Reverse())
            {
                DestroyPage(childPage);
            }
            DestroyPage(page);
        }


        public static Task<bool> CanNavigateAsync(object page, INavigationParameters parameters)
        {
            if (page is IConfirmNavigationAsync confirmNavigationItem)
                return confirmNavigationItem.CanNavigateAsync(parameters);

            if (page is BindableObject bindableObject)
            {
                if (bindableObject.BindingContext is IConfirmNavigationAsync confirmNavigationBindingContext)
                    return confirmNavigationBindingContext.CanNavigateAsync(parameters);
            }

            return Task.FromResult(CanNavigate(page, parameters));
        }

        public static bool CanNavigate(object page, INavigationParameters parameters)
        {
            if (page is IConfirmNavigation confirmNavigationItem)
                return confirmNavigationItem.CanNavigate(parameters);

            if (page is BindableObject bindableObject)
            {
                if (bindableObject.BindingContext is IConfirmNavigation confirmNavigationBindingContext)
                    return confirmNavigationBindingContext.CanNavigate(parameters);
            }

            return true;
        }

        public static void OnNavigatedFrom(object page, INavigationParameters parameters)
        {
            if (page != null)
                InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedFrom(parameters));
        }

        public static void OnNavigatingTo(object page, INavigationParameters parameters)
        {
            if (page != null)
                InvokeViewAndViewModelAction<INavigatingAware>(page, v => v.OnNavigatingTo(parameters));
        }

        public static void OnNavigatedTo(object page, INavigationParameters parameters)
        {
            if (page != null)
                InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedTo(parameters));
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
                if (target != null)
                    target = GetOnNavigatedToTargetFromChild(target);
                else
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

        public static Page GetCurrentPage(Page mainPage)
        {
            var page = mainPage;

            var lastModal = page.Navigation.ModalStack.LastOrDefault();
            if (lastModal != null)
                page = lastModal;

            return GetOnNavigatedToTargetFromChild(page);
        }

        public static void HandleSystemGoBack(Page previousPage, Page currentPage)
        {
            var parameters = new NavigationParameters();
            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);
            OnNavigatedFrom(previousPage, parameters);
            OnNavigatedTo(GetOnNavigatedToTargetFromChild(currentPage), parameters);
            DestroyPage(previousPage);
        }

        internal static bool HasDirectNavigationPageParent(Page page)
        {
            return page?.Parent != null && page?.Parent is NavigationPage;
        }

        internal static bool HasNavigationPageParent(Page page)
        {
            if (page?.Parent != null)
            {
                if (page.Parent is NavigationPage)
                {
                    return true;
                }
                else if (page.Parent is TabbedPage || page.Parent is CarouselPage)
                {
                    return page.Parent.Parent != null && page.Parent.Parent is NavigationPage;
                }
            }

            return false;
        }

        internal static bool IsSameOrSubclassOf<T>(Type potentialDescendant)
        {
            if (potentialDescendant == null)
                return false;

            Type potentialBase = typeof(T);

            return potentialDescendant.GetTypeInfo().IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }

        internal static void SetAutowireViewModelOnPage(Page page)
        {
            var vmlResult = Mvvm.ViewModelLocator.GetAutowireViewModel(page);
            if (vmlResult == null)
                Mvvm.ViewModelLocator.SetAutowireViewModel(page, true);
        }
    }
}
