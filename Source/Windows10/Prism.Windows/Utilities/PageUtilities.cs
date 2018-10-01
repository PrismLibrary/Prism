using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Navigation;
using Windows.UI.Xaml.Controls;

namespace Prism.Utilities
{
    public static class PageUtilities
    {
        public static void InvokeViewAndViewModelAction<T>(object view, Action<T> action) where T : class
        {
            if (view is T viewAsT)
                action(viewAsT);

            if (view is Page page)
            {
                if (page.DataContext is T viewModelAsT)
                {
                    action(viewModelAsT);
                }
            }
        }

        public static async Task InvokeViewAndViewModelActionAsync<T>(object view, Func<T, Task> action)
            where T : class
        {
            if (view is T viewAsT)
                await action(viewAsT);

            if (view is Page page)
            {
                if (page.DataContext is T viewModelAsT)
                {
                    await action(viewModelAsT);
                }
            }
        }

        public static Task<bool> CanNavigateAsync(object page, INavigationParameters parameters)
        {
            if (page is IConfirmNavigationAsync confirmNavigationItem)
                return confirmNavigationItem.CanNavigateAsync(parameters);

            if (page is Page uiPage)
            {
                if (uiPage.DataContext is IConfirmNavigationAsync confirmNavigationBindingContext)
                    return confirmNavigationBindingContext.CanNavigateAsync(parameters);
            }

            return Task.FromResult(CanNavigate(page, parameters));
        }

        public static bool CanNavigate(object page, INavigationParameters parameters)
        {
            if (page is IConfirmNavigation confirmNavigationItem)
                return confirmNavigationItem.CanNavigate(parameters);

            if (page is Page uiPage)
            {
                if (uiPage.DataContext is IConfirmNavigation confirmNavigationBindingContext)
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

        public static async Task OnNavigatedToAsync(object page, INavigationParameters parameters)
        {
            if (page != null)
                await InvokeViewAndViewModelActionAsync<INavigatedAwareAsync>(page, v => v.OnNavigatedToAsync(parameters));
        }
    }
}
