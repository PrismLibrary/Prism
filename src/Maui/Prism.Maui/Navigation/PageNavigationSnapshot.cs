namespace Prism.Navigation;

internal sealed class PageNavigationSnapshot
{
    private readonly Window _window;
    private readonly Page _root;
    private readonly List<Page> _modals;
    private readonly Dictionary<NavigationPage, List<Page>> _navigation = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<FlyoutPage, (Page Detail, bool Presented)> _flyouts = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<TabbedPage, Page> _tabs = new(ReferenceEqualityComparer.Instance);

    internal PageNavigationSnapshot(Window window)
    {
        _window = window;
        _root = window.Page;
        _modals = window.Navigation.ModalStack.ToList();
        Capture(_root);
        foreach (var modal in _modals)
            Capture(modal);
    }

    private void Capture(Page page)
    {
        switch (page)
        {
            case NavigationPage navigation:
                var stack = navigation.Navigation.NavigationStack.ToList();
                if (_navigation.TryAdd(navigation, stack))
                    foreach (var child in stack)
                        Capture(child);
                break;
            case FlyoutPage flyout:
                if (_flyouts.TryAdd(flyout, (flyout.Detail, flyout.IsPresented)))
                    Capture(flyout.Detail);
                break;
            case TabbedPage tabbed:
                if (_tabs.TryAdd(tabbed, tabbed.CurrentPage))
                    foreach (var child in tabbed.Children)
                        Capture(child);
                break;
        }
    }

    internal bool IsCurrent => _window.Page == _root
        && _window.Navigation.ModalStack.SequenceEqual(_modals)
        && _navigation.All(pair => pair.Key.Navigation.NavigationStack.SequenceEqual(pair.Value))
        && _flyouts.All(pair => pair.Key.Detail == pair.Value.Detail)
        && _tabs.All(pair => pair.Key.CurrentPage == pair.Value);

    internal async Task RestoreAsync(Func<INavigation, bool, bool, Task<Page>> pop)
    {
        var commonModals = _window.Navigation.ModalStack.Zip(_modals)
            .TakeWhile(pair => pair.First == pair.Second).Count();
        while (_window.Navigation.ModalStack.Count > commonModals)
        {
            var page = _window.Navigation.ModalStack.Last();
            if (await pop(_window.Navigation, true, false) != page)
                throw new NavigationException(NavigationException.UnsupportedMauiNavigation, page);
        }

        if (_window.Page != _root)
            _window.Page = _root;
        foreach (var (flyout, state) in _flyouts)
        {
            if (flyout.Detail != state.Detail)
                flyout.Detail = state.Detail;
            flyout.IsPresented = state.Presented;
        }
        foreach (var (tabbed, selected) in _tabs)
            tabbed.CurrentPage = selected;

        foreach (var (navigationPage, originalPages) in _navigation)
        {
            var navigation = navigationPage.Navigation;
            for (var index = 0; index < originalPages.Count; index++)
            {
                var page = originalPages[index];
                if (navigation.NavigationStack.Contains(page))
                    continue;
                var next = originalPages.Skip(index + 1).FirstOrDefault(navigation.NavigationStack.Contains);
                if (next is not null)
                    navigation.InsertPageBefore(page, next);
                else
                    await navigation.PushAsync(page, false);
            }
            foreach (var page in navigation.NavigationStack.Reverse().ToArray())
            {
                if (originalPages.Contains(page))
                    continue;
                if (navigationPage.CurrentPage == page)
                {
                    if (await pop(navigation, false, false) != page)
                        throw new NavigationException(NavigationException.UnsupportedMauiNavigation, page);
                }
                else
                    navigation.RemovePage(page);
            }
        }

        foreach (var modal in _modals.Skip(commonModals))
            await _window.Navigation.PushModalAsync(modal, false);
    }
}
