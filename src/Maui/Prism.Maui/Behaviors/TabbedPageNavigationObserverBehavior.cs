using Prism.Events;
using Prism.Navigation;
using Prism.Mvvm;

#nullable enable

namespace Prism.Behaviors;

/// <summary>Publishes changes to the selected tab after the page is attached to a window.</summary>
public class TabbedPageNavigationObserverBehavior : BehaviorBase<TabbedPage>
{
    private readonly IEventAggregator _eventAggregator;
    private Page? _previousTab;

    /// <summary>Creates a behavior using the application's event aggregator.</summary>
    public TabbedPageNavigationObserverBehavior(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
    }

    /// <inheritdoc/>
    protected override void OnAttachedTo(TabbedPage bindable)
    {
        base.OnAttachedTo(bindable);
        _previousTab = bindable.CurrentPage;
        bindable.CurrentPageChanged += OnCurrentPageChanged;
    }

    /// <inheritdoc/>
    protected override void OnDetachingFrom(TabbedPage bindable)
    {
        bindable.CurrentPageChanged -= OnCurrentPageChanged;
        _previousTab = null;
        base.OnDetachingFrom(bindable);
    }

    private void OnCurrentPageChanged(object? sender, EventArgs args)
    {
        var page = AssociatedObject;
        var previousTab = _previousTab;
        var currentTab = page.CurrentPage;
        // Update before publishing so a subscriber can select another tab reentrantly.
        _previousTab = currentTab;
        if (page.Window is null || previousTab is null || currentTab is null || previousTab == currentTab)
            return;

        _eventAggregator.GetEvent<NavigationRequestEvent>().Publish(new NavigationRequestContext
        {
            Type = NavigationRequestType.TabChanged,
            Parameters = new NavigationParameters(),
            Result = new NavigationResult(),
            TabbedPageName = ViewModelLocator.GetNavigationName(page),
            PreviousTabName = GetTabName(previousTab),
            CurrentTabName = GetTabName(currentTab),
        });
    }

    private static string? GetTabName(Page tab)
    {
        var name = ViewModelLocator.GetNavigationName(tab);
        if (tab is not NavigationPage { RootPage: Page root })
            return name;

        var rootName = ViewModelLocator.GetNavigationName(root);
        return name is not null && rootName is not null ? $"{name}|{rootName}" : null;
    }
}
