using System;
using System.Diagnostics;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;
using Prism.Common;
using System.Reflection;
using System.Collections.Generic;

namespace Prism.Navigation
{
    public class PageNavigationService : INavigationService, IPageAware
    {
        private Page _page;
        Page IPageAware.Page
        {
            get { return _page; }
            set { _page = value; }
        }

        public void GoBack(bool useModalNavigation = true, bool animated = true)
        {
            var navigation = GetPageNavigation();
            DoPop(navigation, useModalNavigation, animated);
        }

        public void Navigate<T>(NavigationParameters parameters = null, bool useModalNavigation = true, bool animated = true)
        {
            Navigate(typeof(T).FullName, parameters, useModalNavigation, animated);
        }

        public void Navigate(string name, NavigationParameters parameters = null, bool useModalNavigation = true, bool animated = true)
        {
            var view = ServiceLocator.Current.GetInstance<object>(name) as Page;
            if (view != null)
            {
                Page navigationPageFromProvider = GetNavigationPageFromProvider(view);

                var navigation = GetPageNavigation();

                if (!CanNavigate(_page, parameters))
                    return;

                OnNavigatedFrom(_page, parameters);

                DoPush(navigation, (navigationPageFromProvider != null ? navigationPageFromProvider : view), useModalNavigation, animated);

                OnNavigatedTo(view, parameters);
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

        protected static void OnNavigatedTo(object page, NavigationParameters parameters)
        {
            var currentPage = page as Page;
            if (currentPage != null)
                InvokeOnNavigationAwareElement(page, v => v.OnNavigatedTo(parameters));
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

        static Dictionary<Type, INavigationPageProvider> _navigationProviderCache = new Dictionary<Type, INavigationPageProvider>();

        private Page GetNavigationPageFromProvider(Page view)
        {
            INavigationPageProvider provider = null;
            Type viewType = view.GetType();

            if (_navigationProviderCache.ContainsKey(viewType))
            {
                provider = _navigationProviderCache[viewType];
            }
            else
            {
                var navOptions = viewType.GetTypeInfo().GetCustomAttribute<NavigationPageProviderAttribute>(true);
                if (navOptions != null)
                {
                    provider = ServiceLocator.Current.GetInstance(navOptions.Type) as INavigationPageProvider;
                    if (provider == null)
                        throw new InvalidCastException("Could not create the navigation page provider.  Please make sure the navigation page provider implements the INavigationPageProvider interface.");
                }
            }

            if (!_navigationProviderCache.ContainsKey(viewType))
                _navigationProviderCache.Add(viewType, provider);

            if (provider != null)
            {
                provider.Initialize(view);
                return provider.CreateNavigationPage();
            }

            return null;
        }
    }
}
