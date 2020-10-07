using System;
using System.ComponentModel;
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

        internal static readonly BindableProperty NavigationScopeProperty =
            BindableProperty.CreateAttached("NavigationScope",
                typeof(IScopedProvider),
                typeof(Navigation),
                default(IScopedProvider),
                propertyChanged: OnNavigationScopeChanged);

        private static void OnNavigationScopeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }

            if (oldValue != null && newValue is null && oldValue is IScopedProvider oldProvider)
            {
                oldProvider.Dispose();
                return;
            }

            if (newValue != null && newValue is IScopedProvider scopedProvider)
            {
                scopedProvider.IsAttached = true;
            }
        }

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

        /// <summary>
        /// Gets the Bindable Can Navigate property for an element
        /// </summary>
        /// <param name="view">The bindable element</param>
        public static bool GetCanNavigate(BindableObject view) => (bool)view.GetValue(CanNavigateProperty);

        /// <summary>
        /// Sets the Bindable Can Navigate property for an element
        /// </summary>
        /// <param name="view">The bindable element</param>
        /// <param name="value">The Can Navigate value</param>
        public static void SetCanNavigate(BindableObject view, bool value) => view.SetValue(CanNavigateProperty, value);

        /// <summary>
        /// Gets the instance of <see cref="INavigationService"/> for the given <see cref="Page"/>
        /// </summary>
        /// <param name="page">The <see cref="Page"/></param>
        /// <returns>The <see cref="INavigationService"/></returns>
        /// <remarks>Do not use... this is an internal use API</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INavigationService GetNavigationService(Page page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));

            var navService = (INavigationService)page.GetValue(NavigationServiceProperty);
            if (navService is null)
            {
                var currentScope = (IScopedProvider)page.GetValue(NavigationScopeProperty) ?? ContainerLocator.Container.CurrentScope;

                if (currentScope is null)
                    currentScope = ContainerLocator.Container.CreateScope();

                if (!currentScope.IsAttached)
                    page.SetValue(NavigationScopeProperty, currentScope);

                currentScope.IsAttached = true;

                navService = currentScope.Resolve<INavigationService>();
                if (navService is IPageAware pa)
                {
                    pa.Page = page;
                }

                page.SetValue(NavigationServiceProperty, navService);
            }

            return navService;
        }

        internal static Action GetRaiseCanExecuteChangedInternal(BindableObject view) => (Action)view.GetValue(RaiseCanExecuteChangedInternalProperty);

        internal static void SetRaiseCanExecuteChangedInternal(BindableObject view, Action value) => view.SetValue(RaiseCanExecuteChangedInternalProperty, value);

        private static void OnCanNavigatePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var action = GetRaiseCanExecuteChangedInternal(bindable);
            action?.Invoke();
        }
    }
}
