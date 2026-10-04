namespace Prism.Navigation;

internal sealed class RelativeNavigationState
{
    private readonly Window _window;
    private readonly Page _root;
    private readonly Page[] _modals;
    private readonly HashSet<Page> _ignored;
    private readonly Dictionary<NavigationPage, Page[]> _navigation;
    private readonly Dictionary<Page, Page> _branches;

    internal RelativeNavigationState(Window window, IEnumerable<Page> pages, IEnumerable<Page> ignored = null)
    {
        _window = window;
        _root = window.Page;
        _ignored = new HashSet<Page>(ignored ?? Array.Empty<Page>());
        _modals = Visible(window.Navigation.ModalStack).ToArray();
        var observed = pages.Distinct().ToArray();
        _navigation = observed.OfType<NavigationPage>()
            .ToDictionary(page => page, page => Visible(page.Navigation.NavigationStack).ToArray());
        _branches = observed.Where(page => page is TabbedPage or FlyoutPage)
            .ToDictionary(page => page, GetBranch);
    }

    internal bool IsCurrent => Matches(null, null, null, null);

    internal bool IsCurrentAfterPop(Page page) => Matches(page, null, null, null);

    internal bool IsCurrentAfterPush(Page page, NavigationPage navigation = null, Page before = null) =>
        Matches(null, page, navigation, before);

    private bool Matches(Page removed, Page added, NavigationPage navigation, Page before)
    {
        if (_window.Page != _root || _branches.Any(branch => GetBranch(branch.Key) != branch.Value))
            return false;

        var modals = _modals.Where(page => page != removed);
        if (added is not null && navigation is null)
            modals = modals.Append(added);
        if (!Visible(_window.Navigation.ModalStack).SequenceEqual(modals))
            return false;

        foreach (var (container, pages) in _navigation)
        {
            var expected = pages.Where(page => page != removed).ToList();
            if (added is not null && container == navigation)
                expected.Insert(before is null ? expected.Count : expected.IndexOf(before), added);
            if (!Visible(container.Navigation.NavigationStack).SequenceEqual(expected))
                return false;
        }
        return true;
    }

    private IEnumerable<Page> Visible(IEnumerable<Page> pages) => pages.Where(page => !_ignored.Contains(page));

    private static Page GetBranch(Page page) => page is TabbedPage tabbed ? tabbed.CurrentPage : ((FlyoutPage)page).Detail;
}
