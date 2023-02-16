using Prism.Navigation.Xaml;

namespace Prism.Behaviors;

internal class RegionCleanupBehavior : BehaviorBase<Page>
{
    protected override void OnDetachingFrom(Page bindable)
    {
        bindable.ClearChildRegions();
    }
}
