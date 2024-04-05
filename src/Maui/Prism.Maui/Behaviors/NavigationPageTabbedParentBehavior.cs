using System.ComponentModel;
using Prism.Xaml;

namespace Prism.Behaviors;

/// <summary>
/// Adds a behavior to use the RootPage Title and IconImageSource if they are not set on the NavigaitonPage
/// when the NavigationPage has a TabbedPage parent.
/// </summary>
public sealed class NavigationPageTabbedParentBehavior : BehaviorBase<NavigationPage>
{
    /// <inheritdoc />
    protected override void OnAttachedTo(NavigationPage bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.PropertyChanged += OnRootPageSet;
    }

    private void OnRootPagePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is not Page page || page.Parent is not NavigationPage navigationPage)
        {
            return;
        }

        if (e.PropertyName == DynamicTab.TitleProperty.PropertyName)
        {
            navigationPage.Title = DynamicTab.GetTitle(page);
        }

        if (e.PropertyName == DynamicTab.IconImageSourceProperty.PropertyName)
        {
            navigationPage.IconImageSource = DynamicTab.GetIconImageSource(page);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachingFrom(NavigationPage bindable)
    {
        base.OnDetachingFrom(bindable);
        // Sanity Check
        bindable.PropertyChanged -= OnRootPageSet;
        if (bindable.RootPage is not null)
        {
            bindable.RootPage.PropertyChanged -= OnRootPagePropertyChanged;
        }
    }

    private void OnRootPageSet(object sender, PropertyChangedEventArgs e)
    {
        if (sender is NavigationPage navigationPage && navigationPage.RootPage is not null)
        {
            navigationPage.PropertyChanged -= OnRootPageSet;
            navigationPage.RootPage.PropertyChanged += OnRootPagePropertyChanged;
            navigationPage.Title = DynamicTab.GetTitle(navigationPage.RootPage);
            navigationPage.IconImageSource = DynamicTab.GetIconImageSource(navigationPage.RootPage);
        }
    }
}
