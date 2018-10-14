using System;
using Prism.Common;
using Prism.Ioc;
using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    public static class Navigation
    {
        internal static readonly BindableProperty NavigationServiceProperty =
            BindableProperty.CreateAttached("NavigationService",
                typeof(INavigationService),
                typeof(NavigationExtensionBase),
                default(INavigationService));

        public static readonly BindableProperty CanNavigateProperty =
            BindableProperty.CreateAttached("CanNavigate",
                typeof(bool),
                typeof(NavigationExtensionBase),
                true,
                propertyChanged: OnCanNavigatePropertyChanged);
        
        internal static readonly BindableProperty RaiseCanExecuteChangedInternalProperty =
            BindableProperty.CreateAttached("RaiseCanExecuteChangedInternal",
                typeof(Action),
                typeof(NavigationExtensionBase),
                default(Action));

        public static bool GetCanNavigate(BindableObject view) => (bool) view.GetValue(CanNavigateProperty);

        public static void SetCanNavigate(BindableObject view, bool value) => view.SetValue(CanNavigateProperty, value);

        internal static INavigationService GetNavigationService(Page page)
        {
            if(page == null) throw new ArgumentNullException(nameof(page));

            var navigationService = (INavigationService) page.GetValue(NavigationServiceProperty);
            if (navigationService == null) page.SetValue(NavigationServiceProperty, navigationService = CreateNavigationService(page));

            return navigationService;
        }

        internal static Action GetRaiseCanExecuteChangedInternal(BindableObject view) => (Action) view.GetValue(RaiseCanExecuteChangedInternalProperty);

        internal static void SetRaiseCanExecuteChangedInternal(BindableObject view, Action value) => view.SetValue(RaiseCanExecuteChangedInternalProperty, value);

        private static INavigationService CreateNavigationService(Page view)
        {
            var context = (PrismApplicationBase) Application.Current;
            var navigationService = context.Container.Resolve<INavigationService>("PageNavigationService");
            if (navigationService is IPageAware pageAware) pageAware.Page = view;
            return navigationService;
        }

        private static void OnCanNavigatePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var action = GetRaiseCanExecuteChangedInternal(bindable);
            action?.Invoke();
        }
    }
}