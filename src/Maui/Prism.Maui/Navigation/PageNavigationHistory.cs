namespace Prism.Navigation;

internal static class PageNavigationHistory
{
    internal static IEnumerable<Page> GetPages(Page page)
    {
        if (page is null)
            yield break;

        yield return page;

        IEnumerable<Page> children = page switch
        {
            NavigationPage navigation => navigation.Navigation.NavigationStack,
            TabbedPage tabbed => new[] { tabbed.CurrentPage },
            FlyoutPage flyout => new[] { flyout.Detail },
            _ => Array.Empty<Page>()
        };

        foreach (var child in children)
            foreach (var descendant in GetPages(child))
                yield return descendant;
    }

    internal static bool IsDescendantOf(Page page, Page ancestor)
    {
        for (var parent = page.Parent; parent is not null; parent = parent.Parent)
            if (parent == ancestor)
                return true;

        return false;
    }

    internal static Page GetPreviousPage(Page page, Page root, IReadOnlyList<Page> modalStack)
    {
        while (page is not null)
        {
            if (page.Parent is NavigationPage navigation)
            {
                var stack = navigation.Navigation.NavigationStack;
                var index = stack.ToList().IndexOf(page);
                if (index > 0)
                    return Prism.Common.MvvmHelpers.GetTarget(stack[index - 1]);
            }

            var modalIndex = modalStack.ToList().IndexOf(page);
            if (modalIndex >= 0)
                return Prism.Common.MvvmHelpers.GetTarget(modalIndex == 0 ? root : modalStack[modalIndex - 1]);

            page = page.Parent as Page;
        }

        return null;
    }
}
