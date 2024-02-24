using Prism.Common;
using Prism.Extensions;

namespace Prism.Behaviors;

/// <summary>
/// Provides behaviors for types of <see cref="MultiPage{T}"/>
/// </summary>
/// <typeparam name="T">The typeof <see cref="Page"/>.</typeparam>
public class MultiPageActiveAwareBehavior<T> : BehaviorBase<MultiPage<T>> where T : Page
{
    /// <inheritDoc/>
    protected override void OnAttachedTo(MultiPage<T> bindable)
    {
        bindable.CurrentPageChanged += CurrentPageChangedHandler;
        bindable.Appearing += RootPageAppearingHandler;
        bindable.Disappearing += RootPageDisappearingHandler;
        base.OnAttachedTo(bindable);
    }

    /// <inheritDoc/>
    protected override void OnDetachingFrom(MultiPage<T> bindable)
    {
        bindable.CurrentPageChanged -= CurrentPageChangedHandler;
        bindable.Appearing -= RootPageAppearingHandler;
        bindable.Disappearing -= RootPageDisappearingHandler;
        base.OnDetachingFrom(bindable);
    }

    /// <summary>
    /// Event Handler for the MultiPage CurrentPageChanged event
    /// </summary>
    /// <param name="sender">The MultiPage</param>
    /// <param name="e">Event Args</param>
    protected void CurrentPageChangedHandler(object sender, EventArgs e)
    {
        SetActiveAware();
    }

    /// <summary>
    /// Event Handler for the MultiPage Appearing event
    /// </summary>
    /// <param name="sender">The MultiPage Appearing</param>
    /// <param name="e">Event Args</param>
    protected void RootPageAppearingHandler(object sender, EventArgs e)
    {
        SetActiveAware();
    }

    /// <summary>
    /// Event Handler for the MultiPage Disappearing event
    /// </summary>
    /// <param name="sender">The MultiPage Disappearing</param>
    /// <param name="e">Event Args</param>
    protected void RootPageDisappearingHandler(object sender, EventArgs e)
    {
        SetActiveAware();
    }

    private void SetActiveAware()
    {
        AssociatedObject.Children.ForEach(page => SetPageIsActive(page, AssociatedObject.CurrentPage == page));
    }

    private void SetPageIsActive(Page page, bool isActive)
    {
        MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(page, aa => SetIsActive(aa, isActive));

        if (page is NavigationPage navPage)
        {
            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(navPage.CurrentPage, aa => SetIsActive(aa, isActive));
        }
    }

    private static void SetIsActive(IActiveAware activeAware, bool isActive)
    {
        if (activeAware?.IsActive != isActive)
            activeAware.IsActive = isActive;
    }
}
