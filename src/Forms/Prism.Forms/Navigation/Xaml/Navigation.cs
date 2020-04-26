using System;
using Prism.Common;
using Prism.Ioc;
using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    /// <summary>
    /// Provides Attachable properties for Navigation
    /// </summary>
    public static class Navigation
    {
        internal static readonly BindableProperty NavigationServiceProperty =
            BindableProperty.CreateAttached("NavigationService",
                typeof(INavigationService),
                typeof(Navigation),
                default(INavigationService));

        /// <summary>
        /// Provides bindable CanNavigate Bindable Property
        /// </summary>
        public static readonly BindableProperty CanNavigateProperty =
            BindableProperty.CreateAttached("CanNavigate",
                typeof(bool),
                typeof(Navigation),
                true,
                propertyChanged: OnCanNavigatePropertyChanged);
        
        internal static readonly BindableProperty RaiseCanExecuteChangedInternalProperty =
            BindableProperty.CreateAttached("RaiseCanExecuteChangedInternal",
                typeof(Action),
                typeof(Navigation),
                default(Action));

        public static bool GetCanNavigate(BindableObject view) => (bool) view.GetValue(CanNavigateProperty);

        public static void SetCanNavigate(BindableObject view, bool value) => view.SetValue(CanNavigateProperty, value);

        internal static INavigationService GetNavigationService(Page page)
        {
            if(page == null) throw new ArgumentNullException(nameof(page));

            return (INavigationService) page.GetValue(NavigationServiceProperty);
        }

        internal static Action GetRaiseCanExecuteChangedInternal(BindableObject view) => (Action) view.GetValue(RaiseCanExecuteChangedInternalProperty);

        internal static void SetRaiseCanExecuteChangedInternal(BindableObject view, Action value) => view.SetValue(RaiseCanExecuteChangedInternalProperty, value);

        private static void OnCanNavigatePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var action = GetRaiseCanExecuteChangedInternal(bindable);
            action?.Invoke();
        }
    }
}