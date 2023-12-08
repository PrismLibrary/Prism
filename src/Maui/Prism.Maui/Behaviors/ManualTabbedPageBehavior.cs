using Prism.Common;

namespace Prism.Behaviors;

/// <summary>
/// Ensures that a Container Scope is set for any Pages that have been manually added to a TabbedPage
/// </summary>
public class ManualTabbedPageBehavior : BehaviorBase<TabbedPage>
{
    private readonly IContainerExtension _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualTabbedPageBehavior"/>
    /// </summary>
    /// <param name="container">The root <see cref="IContainerExtension"/></param>
    public ManualTabbedPageBehavior(IContainerExtension container) =>
        _container = container;

    /// <inheritdoc />
    protected override void OnAttachedTo(TabbedPage tabbedPage)
    {
        foreach(var child in tabbedPage.Children)
        {
            EnsureContainerScopeIsSet(child);
        }
        base.OnAttachedTo(tabbedPage);
    }

    private void EnsureContainerScopeIsSet(Page page)
    {
        var container = Navigation.Xaml.Navigation.GetContainerProvider(page);
        if (container is null)
        {
            var scope = _container.CreateScope();
            var accessor = scope.Resolve<IPageAccessor>();
            accessor.Page = page;
            Navigation.Xaml.Navigation.SetContainerProvider(page, scope);
        }

        if (page is NavigationPage navigationPage)
        {
            EnsureContainerScopeIsSet(navigationPage.RootPage);
        }
    }
}
