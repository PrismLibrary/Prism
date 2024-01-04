namespace Prism.Behaviors;

/// <summary>
/// Adds a behavior to use the RootPage Title and IconImageSource if they are not set on the NavigaitonPage
/// when the NavigationPage has a TabbedPage parent.
/// </summary>
public sealed class NavigationPageTabbedParentBehavior : BehaviorBase<NavigationPage>
{
    private static readonly BindableProperty NavigationPageRootPageMonitorTitleProperty =
        BindableProperty.CreateAttached("NavigationPageRootPageMonitorTitle", typeof(bool), typeof(NavigationPageTabbedParentBehavior), false);

    private static readonly BindableProperty NavigationPageRootPageMonitorIconImageSourceProperty =
        BindableProperty.CreateAttached("NavigationPageRootPageMonitorIconImageSource", typeof(bool), typeof(NavigationPageTabbedParentBehavior), false);

    private static bool GetNavigationPageRootPageMonitorTitle(BindableObject bindable) =>
        (bool)bindable.GetValue(NavigationPageRootPageMonitorTitleProperty);

    private static void SetNavigationPageRootPageMonitorTitle(BindableObject bindable, bool monitorTitle) =>
        bindable.SetValue(NavigationPageRootPageMonitorTitleProperty, monitorTitle);

    private static bool GetNavigationPageRootPageMonitorIconImageSource(BindableObject bindable) =>
        (bool)bindable.GetValue(NavigationPageRootPageMonitorIconImageSourceProperty);

    private static void SetNavigationPageRootPageMonitorIconImageSource(BindableObject bindable, bool monitorTitle) =>
        bindable.SetValue(NavigationPageRootPageMonitorIconImageSourceProperty, monitorTitle);

    /// <inheritdoc />
    protected override void OnAttachedTo(NavigationPage bindable)
    {
        base.OnAttachedTo(bindable);
        SetNavigationPageRootPageMonitorTitle(bindable, !bindable.IsSet(NavigationPage.TitleProperty));
        SetNavigationPageRootPageMonitorIconImageSource(bindable, !bindable.IsSet(NavigationPage.IconImageSourceProperty));
        bindable.ParentChanged += OnParentChanged;
    }

    /// <inheritdoc />
    protected override void OnDetachingFrom(NavigationPage bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.ParentChanged -= OnParentChanged;
    }

    private void OnParentChanged(object sender, EventArgs e)
    {
        if (sender is not NavigationPage navigationPage || navigationPage.Parent is not TabbedPage)
            return;

        if (GetNavigationPageRootPageMonitorTitle(navigationPage))
            navigationPage.SetBinding(NavigationPage.TitleProperty, new Binding("RootPage.Title", BindingMode.OneWay, source: navigationPage));

        if (GetNavigationPageRootPageMonitorIconImageSource(navigationPage))
            navigationPage.SetBinding(NavigationPage.IconImageSourceProperty, new Binding("RootPage.IconImageSource", BindingMode.OneWay, source: navigationPage));
    }
}
