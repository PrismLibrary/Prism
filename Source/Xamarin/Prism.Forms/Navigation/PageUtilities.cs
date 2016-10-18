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

        public static void DisposePage(Page page)
        {
            try
            {
                DisposeChildren(page);

                var viewModel = page.BindingContext as IDisposable;
                if (viewModel != null)
                    viewModel.Dispose();

                page.ToolbarItems?.Clear();
                page.Behaviors?.Clear();
                page.BindingContext = null;

                var disposablePage = page as IDisposable;
                if (disposablePage != null)
                    disposablePage.Dispose();
            }
            catch (ObjectDisposedException ex)
            {
                throw new ObjectDisposedException($"Cannot dipose {page}: {ex.Message}");
            }
        }

        private static void DisposeChildren(Page page)
        {
            if (page is MasterDetailPage)
            {
                DisposePage(((MasterDetailPage)page).Detail);
            }
            else if (page is TabbedPage)
            {
                var tabbedPage = (TabbedPage)page;
                foreach (var item in tabbedPage.Children)
                {
                    DisposePage(item);
                }
            }
            else if (page is CarouselPage)
            {
                var carouselPage = (CarouselPage)page;
                foreach (var item in carouselPage.Children)
                {
                    DisposePage(item);
                }
            }
            else if (page is NavigationPage)
            {
                var navigationPage = (NavigationPage)page;
                foreach (var item in navigationPage.Navigation.NavigationStack)
                {
                    DisposePage(item);
                }
            }
        }
    }
}
