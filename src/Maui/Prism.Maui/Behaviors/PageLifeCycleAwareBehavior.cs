using Prism.AppModel;
using Prism.Common;

namespace Prism.Behaviors;

/// <summary>
/// Provides lifecycle events for <see cref="Page"/> and <see cref="IPageLifecycleAware"/> ViewModels.
/// </summary>
public class PageLifeCycleAwareBehavior : BehaviorBase<Page>
{
    /// <inheritdoc />
    protected override void OnAttachedTo(Page bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.Appearing += OnAppearing;
        bindable.Disappearing += OnDisappearing;
    }

    /// <inheritdoc />
    protected override void OnDetachingFrom(Page bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.Appearing -= OnAppearing;
        bindable.Disappearing -= OnDisappearing;
    }

    private void OnAppearing(object sender, EventArgs e)
    {
        MvvmHelpers.InvokeViewAndViewModelAction<IPageLifecycleAware>(AssociatedObject, aware => aware.OnAppearing());
    }

    private void OnDisappearing(object sender, EventArgs e)
    {
        MvvmHelpers.InvokeViewAndViewModelAction<IPageLifecycleAware>(AssociatedObject, aware => aware.OnDisappearing());
    }
}
