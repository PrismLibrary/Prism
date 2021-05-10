using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Prism.AppModel;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Common
{
    public static class PageUtilities
    {
        public static void InvokeViewAndViewModelAction<T>(object view, Action<T> action) where T : class
        {
            if (view is T viewAsT)
            {
                action(viewAsT);
            }

            if (view is BindableObject element && element.BindingContext is T viewModelAsT)
            {
                action(viewModelAsT);
            }
        }

        public static async Task InvokeViewAndViewModelActionAsync<T>(object view, Func<T, Task> action) where T : class
        {
            if (view is T viewAsT)
            {
                await action(viewAsT);
            }

            if (view is BindableObject element && element.BindingContext is T viewModelAsT)
            {
                await action(viewModelAsT);
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
            switch (page)
            {
                case FlyoutPage flyout:
                    DestroyPage(flyout.Flyout);
                    DestroyPage(flyout.Detail);
                    break;
                case TabbedPage tabbedPage:
                    foreach (var item in tabbedPage.Children.Reverse())
                    {
                        DestroyPage(item);
                    }
                    break;
                case CarouselPage carouselPage:
                    foreach (var item in carouselPage.Children.Reverse())
                    {
                        DestroyPage(item);
                    }
                    break;
                case NavigationPage navigationPage:
                    foreach (var item in navigationPage.Navigation.NavigationStack.Reverse())
                    {
                        DestroyPage(item);
                    }
                    break;
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

        public static async Task OnInitializedAsync(object page, INavigationParameters parameters)
        {
            if (page is null) return;

            InvokeViewAndViewModelAction<IInitialize>(page, v => v.Initialize(parameters));
            await InvokeViewAndViewModelActionAsync<IInitializeAsync>(page, async v => await v.InitializeAsync(parameters));
        }

        private static bool HasKey(this IEnumerable<KeyValuePair<string, object>> parameters, string name, out string key)
        {
            key = parameters.Select(x => x.Key).FirstOrDefault(k => k.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return !string.IsNullOrEmpty(key);
        }

        public static void OnNavigatedTo(object page, INavigationParameters parameters)
        {
            if (page != null)
                InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedTo(parameters));
        }

        public static Page GetOnNavigatedToTarget(Page page, Page mainPage, bool useModalNavigation)
        {
            Page target;
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

            if (target is FlyoutPage flyout)
                child = flyout.Detail;
            else if (target is TabbedPage tabbed)
                child = tabbed.CurrentPage;
            else if (target is CarouselPage carousel)
                child = carousel.CurrentPage;
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

        public static Page GetCurrentPage(Page mainPage) =>
            _getCurrentPage(mainPage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetCurrentPageDelegate(Func<Page, Page> getCurrentPageDelegate) =>
            _getCurrentPage = getCurrentPageDelegate;

        private static Func<Page, Page> _getCurrentPage = mainPage =>
        {
            var page = mainPage;

            var lastModal = page.Navigation.ModalStack.LastOrDefault();
            if (lastModal != null)
                page = lastModal;

            return GetOnNavigatedToTargetFromChild(page);
        };

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

        internal static bool HasNavigationPageParent(Page page) =>
            HasNavigationPageParent(page, out var _);

        internal static bool HasNavigationPageParent(Page page, out NavigationPage navigationPage)
        {
            if (page?.Parent != null)
            {
                if (page.Parent is NavigationPage navParent)
                {
                    navigationPage = navParent;
                    return true;
                }
                else if ((page.Parent is TabbedPage || page.Parent is CarouselPage) && page.Parent?.Parent is NavigationPage navigationParent)
                {
                    navigationPage = navigationParent;
                    return true;
                }
            }

            navigationPage = null;
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

        /// <summary>
        /// Sets the AutowireViewModel property on the View to <c>true</c> if there is currently
        /// no BindingContext and the AutowireViewModel property has not been set.
        /// </summary>
        /// <param name="element">The View typically a <see cref="Page"/> or <see cref="View"/>.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetAutowireViewModel(VisualElement element)
        {
            if (element.IsSet(ViewModelLocator.AutowireViewModelProperty))
            {
                return;
            }

            if (element.BindingContext is null)
            {
                ViewModelLocator.SetAutowireViewModel(element, true);
            }
            else if (element.BindingContext == element.Parent?.BindingContext)
            {
                //if the parent binding context is the same as the element, then it was probably inherited
                //and we don't want that. Set the VML
                ViewModelLocator.SetAutowireViewModel(element, true);
            }
        }
    }
}
