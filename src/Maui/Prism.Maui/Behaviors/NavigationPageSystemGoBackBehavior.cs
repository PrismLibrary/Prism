using Prism.Common;
using Prism.Navigation;

namespace Prism.Behaviors;

/// <summary>Notifies Prism navigation callbacks when a navigation page is popped by the platform.</summary>
/// <remarks>Use PrismNavigationPage to integrate platform back navigation with Prism automatically.</remarks>
public class NavigationPageSystemGoBackBehavior : BehaviorBase<NavigationPage>
{
    /// <inheritdoc/>
    protected override void OnAttachedTo(NavigationPage bindable)
    {
        bindable.Popped += NavigationPage_Popped;
        base.OnAttachedTo(bindable);
    }

    /// <inheritdoc/>
    protected override void OnDetachingFrom(NavigationPage bindable)
    {
        bindable.Popped -= NavigationPage_Popped;
        base.OnDetachingFrom(bindable);
    }

    private void NavigationPage_Popped(object sender, NavigationEventArgs e)
    {
        if (PageNavigationService.NavigationSource == PageNavigationSource.Device)
        {
            System.Diagnostics.Trace.WriteLine("NavigationPage has encountered an unhandled GoBack. Be sure to inherit from PrismNavigationPage.");
            MvvmHelpers.HandleSystemGoBack(e.Page, AssociatedObject.CurrentPage);
        }
    }
}
