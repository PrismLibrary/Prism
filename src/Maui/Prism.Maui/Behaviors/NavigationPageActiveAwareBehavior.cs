using System.ComponentModel;
using Prism.Common;
using Prism.Extensions;

namespace Prism.Behaviors;

/// <summary>
/// Provides activation and deactivation for <see cref="IActiveAware"/> ViewModels that may be set as the CurrentPage.
/// </summary>
public class NavigationPageActiveAwareBehavior : BehaviorBase<NavigationPage>
{
    /// <inheritDoc/>
    protected override void OnAttachedTo(NavigationPage bindable)
    {
        bindable.PropertyChanged += NavigationPage_PropertyChanged;
        if (bindable.Parent is null)
            bindable.ParentChanged += OnParentChanged;
        base.OnAttachedTo(bindable);
    }

    private void OnParentChanged(object sender, EventArgs e)
    {
        AssociatedObject.ParentChanged -= OnParentChanged;
        SetActiveAware();
    }

    /// <inheritDoc/>
    protected override void OnDetachingFrom(NavigationPage bindable)
    {
        bindable.PropertyChanged -= NavigationPage_PropertyChanged;
        base.OnDetachingFrom(bindable);
    }

    private void NavigationPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "CurrentPage")
        {
            SetActiveAware();
        }
    }

    private void SetActiveAware()
    {
        if(AssociatedObject.Parent is TabbedPage tabbed && tabbed.CurrentPage != AssociatedObject)
        {
            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(AssociatedObject, SetNotActive);
            AssociatedObject.Navigation.NavigationStack.ForEach(page => MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(page, SetNotActive));
        }
        else
        {
            AssociatedObject.Navigation.NavigationStack.ForEach(page =>
            {
                if (page != AssociatedObject.CurrentPage)
                    MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(page, SetNotActive);
                else
                    MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(page, SetIsActive);
            });
        }
    }

    private void SetNotActive(IActiveAware activeAware)
    {
        if (activeAware.IsActive)
            activeAware.IsActive = false;
    }

    private void SetIsActive(IActiveAware activeAware)
    {
        if(!activeAware.IsActive)
            activeAware.IsActive = true;
    }
}
