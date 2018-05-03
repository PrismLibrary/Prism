using System;
using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    public static class Navigation
    {
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
        
        internal static Action GetRaiseCanExecuteChangedInternal(BindableObject view) => (Action) view.GetValue(RaiseCanExecuteChangedInternalProperty);

        internal static void SetRaiseCanExecuteChangedInternal(BindableObject view, Action value) => view.SetValue(RaiseCanExecuteChangedInternalProperty, value);

        private static void OnCanNavigatePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var action = GetRaiseCanExecuteChangedInternal(bindable);
            action?.Invoke();
        }
    }
}