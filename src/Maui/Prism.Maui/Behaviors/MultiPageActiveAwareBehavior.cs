using Prism.Common;

namespace Prism.Behaviors;

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
        foreach (var page in AssociatedObject.Children)
        {
            SetPageIsActive(page);
        }
    }

    private void SetPageIsActive(Page page)
    {
        if(AssociatedObject.CurrentPage == page)
        {
            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(page, SetIsActive);
            if (page is NavigationPage navPage)
                MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(navPage.CurrentPage, SetIsActive);
        }
        else
        {
            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(page, SetNotActive);
            if (page is NavigationPage navPage)
                MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(navPage.CurrentPage, SetNotActive);
        }
    }

    private void SetNotActive(IActiveAware activeAware)
    {
        if (activeAware.IsActive)
            activeAware.IsActive = false;
    }

    private void SetIsActive(IActiveAware activeAware)
    {
        if (!activeAware.IsActive)
            activeAware.IsActive = true;
    }
}
