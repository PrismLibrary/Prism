using Prism.Navigation.Xaml;

namespace Prism.Behaviors;

/// <summary>
/// Controls the Page container Scope
/// </summary>
public sealed class PageScopeBehavior : BehaviorBase<Page>
{
    /// <inheritdoc />
    protected override void OnAttachedTo(Page page)
    {
        base.OnAttachedTo(page);
        // Ensure the scope gets created and NavigationService is created
        Navigation.Xaml.Navigation.GetNavigationService(page);
    }

    /// <inheritdoc />
    protected override void OnDetachingFrom(Page page)
    {
        base.OnDetachingFrom(page);
        // This forces the Attached Property to get cleaned up.
        page.SetContainerProvider(null);
    }
}
