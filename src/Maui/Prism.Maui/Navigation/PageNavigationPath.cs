using Prism.Mvvm;

namespace Prism.Navigation;

internal static class PageNavigationPath
{
    internal static string GetPath(Window window, Page target)
    {
        var segments = new List<string>();
        if (Append(window.Page, target, segments))
            return "/" + string.Join("/", segments);

        foreach (var modal in window.Navigation.ModalStack)
        {
            var start = segments.Count;
            var found = Append(modal, target, segments);
            if (segments.Count > start)
                segments[start] += segments[start].Contains('?') ? "&" : "?";
            if (segments.Count > start)
                segments[start] += $"{KnownNavigationParameters.UseModalNavigation}=true";
            if (found)
                return "/" + string.Join("/", segments);
        }

        return string.Empty;
    }

    // Include the preceding stack entries but stop at the scoped page. An inactive
    // tab's service must describe that tab, rather than the window's selected tab.
    private static bool Append(Page page, Page target, List<string> segments)
    {
        if (page is null)
            return false;

        var name = ViewModelLocator.GetNavigationName(page);
        segments.Add(name);
        if (ReferenceEquals(page, target))
            return true;

        switch (page)
        {
            case NavigationPage navigation:
                foreach (var child in navigation.Navigation.NavigationStack)
                    if (Append(child, target, segments))
                        return true;
                break;
            case FlyoutPage flyout:
                return Append(flyout.Detail, target, segments);
            case TabbedPage tabbed:
                var tab = tabbed.Children.FirstOrDefault(child =>
                    ReferenceEquals(child, target) || PageNavigationHistory.IsDescendantOf(target, child))
                    ?? tabbed.CurrentPage;
                if (tab is not null)
                {
                    var tabName = ViewModelLocator.GetNavigationName(tab);
                    if (tab is NavigationPage nav && nav.RootPage is not null)
                        tabName += $"|{ViewModelLocator.GetNavigationName(nav.RootPage)}";
                    segments[^1] += $"?{KnownNavigationParameters.SelectedTab}={Uri.EscapeDataString(tabName)}";
                    // The selectedTab parameter represents the tab's root; retain
                    // subsequent pushes so the path still describes its back stack.
                    if (tab is NavigationPage tabNavigation)
                    {
                        if (ReferenceEquals(tab, target))
                            return true;
                        foreach (var child in tabNavigation.Navigation.NavigationStack)
                        {
                            if (ReferenceEquals(child, tabNavigation.RootPage))
                            {
                                if (ReferenceEquals(child, target))
                                    return true;
                            }
                            else if (Append(child, target, segments))
                                return true;
                        }
                    }
                    else
                        return ReferenceEquals(tab, target);
                }
                break;
        }

        return false;
    }
}
