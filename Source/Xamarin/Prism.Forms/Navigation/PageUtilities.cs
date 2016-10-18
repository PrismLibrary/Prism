using System;
using Xamarin.Forms;

namespace Prism.Navigation
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
                DisposeChildren(page);

                var viewModel = page.BindingContext as IDestroy;
                viewModel?.Destroy();

                page.Behaviors?.Clear();
                page.BindingContext = null;

                var destroyPage = page as IDestroy;
                destroyPage?.Destroy();
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot destroy {page}: {ex.Message}");
            }
        }

        private static void DisposeChildren(Page page)
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

        public static void OnNavigatedTo(object page, NavigationParameters parameters)
        {
            if (page != null)
                InvokeViewAndViewModelAction<INavigationAware>(page, v => v.OnNavigatedTo(parameters));
        }
    }
}
