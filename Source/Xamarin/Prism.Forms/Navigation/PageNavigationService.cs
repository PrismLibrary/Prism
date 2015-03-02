using System;
using System.Diagnostics;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;

namespace Prism.Navigation
{
    public class PageNavigationService : INavigationService
    {
        //TODO:  keep internal stack for instance navigation
        //TODO:  need ability to hook into Popping event to see if a view can be popped and cancel the action if it can't.

        public void GoBack(bool animated = true, bool useModalNavigation = true)
        {
            GoBack(null, new NavigationParameters(), animated, useModalNavigation);
        }

        public void GoBack(object page, NavigationParameters parameters, bool animated = true, bool useModalNavigation = true)
        {
            var navigation = GetPageNavigation(page);

            if (!CanNavigate(page, parameters))
                return;

            OnNavigatedFrom(page, parameters);

            DoPop(navigation, animated, useModalNavigation);
        }

        public void Navigate(string name, bool animated = true, bool useModalNavigation = true)
        {
            Navigate(null, name, new NavigationParameters(), animated, useModalNavigation);
        }

        public void Navigate(object page, string name, NavigationParameters parameters, bool animated = true, bool useModalNavigation = true)
        {
            var view = ServiceLocator.Current.GetInstance<object>(name) as Page;
            if (view != null)
            {
                var navigation = GetPageNavigation(page);

                if (!CanNavigate(page, parameters))
                    return;

                OnNavigatedFrom(page, parameters);

                DoPush(navigation, view, animated, useModalNavigation);

                InvokeOnNavigationAwareElement(view, v => v.OnNavigatedTo(parameters));
            }
            else
                Debug.WriteLine("Navigation ERROR: {0} not found. Make sure you have registered {0} for navigation.", name);
        }

        public void Remove(object page)
        {
            var currentPage = page as Page;
            if (currentPage == null) return;

            var navigation = currentPage.Navigation;

            InvokeOnNavigationAwareElement(currentPage, v => v.OnNavigatedFrom(new NavigationParameters()));
            navigation.RemovePage(currentPage);
        }

        private async static void DoPush(INavigation navigation, Page view, bool animated, bool useModalNavigation)
        {
            if (useModalNavigation)
                await navigation.PushModalAsync(view, animated);
            else
                await navigation.PushAsync(view, animated);
        }

        private async static void DoPop(INavigation navigation, bool animated, bool useModalNavigation)
        {
            if (useModalNavigation)
                await navigation.PopModalAsync(animated);
            else
                await navigation.PopAsync(animated);
        }

        private static INavigation GetPageNavigation(object page)
        {
            var currentPage = page as Page;
            return currentPage != null ? currentPage.Navigation : Application.Current.MainPage.Navigation;
        }

        private static void OnNavigatedFrom(object page, NavigationParameters parameters)
        {
            var currentPage = page as Page;
            if (currentPage != null)
                InvokeOnNavigationAwareElement(currentPage, v => v.OnNavigatedFrom(parameters));
        }

        private static bool CanNavigate(object item, NavigationParameters parameters)
        {
            var confirmNavigationItem = item as IConfirmNavigation;
            if (confirmNavigationItem != null)
            {
                return confirmNavigationItem.CanNavigate(parameters);
            }

            var bindableObject = item as BindableObject;
            if (bindableObject != null)
            {
                var confirmNavigationBindingContext = bindableObject.BindingContext as IConfirmNavigation;
                if (confirmNavigationBindingContext != null)
                {
                    return confirmNavigationBindingContext.CanNavigate(parameters);
                }
            }

            return true;
        }

        private static void InvokeOnNavigationAwareElement(object item, Action<INavigationAware> invocation)
        {
            var navigationAwareItem = item as INavigationAware;
            if (navigationAwareItem != null)
            {
                invocation(navigationAwareItem);
            }

            var bindableObject = item as BindableObject;
            if (bindableObject != null)
            {
                var navigationAwareDataContext = bindableObject.BindingContext as INavigationAware;
                if (navigationAwareDataContext != null)
                {
                    invocation(navigationAwareDataContext);
                }
            }
        }
    }
}
