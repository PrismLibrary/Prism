using System;
using System.Diagnostics;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;

namespace Prism.Navigation
{
    public class PageNavigationService : INavigationService
    {
        internal Page Page { get; set; }

        public void GoBack(bool animated = true, bool useModalNavigation = true)
        {
            GoBack(new NavigationParameters(), animated, useModalNavigation);
        }

        public void GoBack(NavigationParameters parameters, bool animated = true, bool useModalNavigation = true)
        {
            var navigation = GetPageNavigation();

            if (!CanNavigate(Page, parameters))
                return;

            OnNavigatedFrom(Page, parameters);

            DoPop(navigation, animated, useModalNavigation);
        }

        public void Navigate(string name, bool animated = true, bool useModalNavigation = true)
        {
            Navigate(name, new NavigationParameters(), animated, useModalNavigation);
        }

        public void Navigate(string name, NavigationParameters parameters, bool animated = true, bool useModalNavigation = true)
        {
            var view = ServiceLocator.Current.GetInstance<object>(name) as Page;
            if (view != null)
            {
                var navigation = GetPageNavigation();

                if (!CanNavigate(Page, parameters))
                    return;

                OnNavigatedFrom(Page, parameters);

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

        private INavigation GetPageNavigation()
        {
            return Page.Navigation ?? Application.Current.MainPage.Navigation;
        }

        protected static bool CanNavigate(object item, NavigationParameters parameters)
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

        protected static void OnNavigatedFrom(object page, NavigationParameters parameters)
        {
            var currentPage = page as Page;
            if (currentPage != null)
                InvokeOnNavigationAwareElement(currentPage, v => v.OnNavigatedFrom(parameters));
        }

        protected static void InvokeOnNavigationAwareElement(object item, Action<INavigationAware> invocation)
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
