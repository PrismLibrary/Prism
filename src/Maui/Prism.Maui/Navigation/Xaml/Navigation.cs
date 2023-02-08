using System.ComponentModel;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation.Internals;

namespace Prism.Navigation.Xaml;

/// <summary>
/// Provides Attachable properties for Navigation
/// </summary>
public static class Navigation
{
    internal const string PrismContainerProvider = nameof(PrismContainerProvider);

    private static readonly BindableProperty NavigationScopeProperty =
        BindableProperty.CreateAttached(PrismContainerProvider,
            typeof(IContainerProvider),
            typeof(Navigation),
            default(IContainerProvider),
            propertyChanged: OnNavigationScopeChanged);

    private static readonly BindableProperty ChildMvvmViewsProperty =
        BindableProperty.CreateAttached("ChildRegions",
            typeof(ChildRegionCollection),
            typeof(Navigation),
            null);

    private static void OnNavigationScopeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Page page || oldValue == newValue)
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
            var accessor = scopedProvider.Resolve<IPageAccessor>();
            if (accessor.Page is null)
                accessor.Page = page;
            else if (accessor.Page != page)
                throw new InvalidOperationException($"The Scoped Provider has already been assigned to another page. Expected: '{page.GetType().FullName}' - Found: '{accessor.Page.GetType().FullName}'.");

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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ChildRegionCollection GetChildRegions(this Page page, bool setIfNull = false)
    {
        var value = page.GetValue(ChildMvvmViewsProperty) as ChildRegionCollection;
        if (value is null && setIfNull)
        {
            value = new ChildRegionCollection();
            page.SetValue(ChildMvvmViewsProperty, value);
        }

        return value;
    }

    internal static void ClearChildRegions(this Page page)
    {
        var children = page.GetChildRegions();
        if(children is not null)
        {
            children.Dispose();
            page.SetValue(ChildMvvmViewsProperty, null);
        }
    }

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

        var container = page.GetContainerProvider();
        return container.Resolve<INavigationService>();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetContainerProvider(this BindableObject bindable, IContainerProvider container)
    {
        bindable.SetValue(NavigationScopeProperty, container);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IContainerProvider GetContainerProvider(this BindableObject bindable)
    {
        if (bindable is null)
            return null;

        var container = bindable.GetValue(NavigationScopeProperty) as IContainerProvider;
        if (container is not null)
            return container;
        else if(bindable is Page page)
        {
            if (page.Parent is FlyoutPage flyout && flyout.Flyout == page)
                return flyout.GetContainerProvider();
        }
        else if (bindable is Element element && element.Parent is not null)
            return GetContainerProvider(element.Parent);

        return null;
    }

    internal static Action GetRaiseCanExecuteChangedInternal(BindableObject view) => (Action)view.GetValue(RaiseCanExecuteChangedInternalProperty);

    internal static void SetRaiseCanExecuteChangedInternal(BindableObject view, Action value) => view.SetValue(RaiseCanExecuteChangedInternalProperty, value);

    private static void OnCanNavigatePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var action = GetRaiseCanExecuteChangedInternal(bindable);
        action?.Invoke();
    }
}
