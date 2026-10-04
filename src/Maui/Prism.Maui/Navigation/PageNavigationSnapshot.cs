namespace Prism.Navigation;

internal sealed class PageNavigationSnapshot
{
    private readonly Window _window;
    private Page _root;
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

    internal bool CanRestore(IReadOnlyCollection<Page> createdPages) =>
        (_window.Page == _root || createdPages.Contains(_window.Page))
        && _window.Navigation.ModalStack.All(page => _modals.Contains(page) || createdPages.Contains(page))
        && _navigation.All(pair => pair.Key.Navigation.NavigationStack.All(page =>
            pair.Value.Contains(page) || createdPages.Contains(page)))
        && _flyouts.All(pair => pair.Key.Detail == pair.Value.Detail || createdPages.Contains(pair.Key.Detail));

    internal async Task RestoreAsync(Func<INavigation, bool, bool, Task<Page>> pop, IReadOnlyCollection<Page> createdPages)
    {
        var expected = new PageNavigationSnapshot(_window);
        foreach (var navigation in _navigation.Keys)
            expected.Capture(navigation);
        foreach (var flyout in _flyouts.Keys)
            expected.Capture(flyout);
        foreach (var tabbed in _tabs.Keys)
            expected.Capture(tabbed);

        void EnsureOwnership()
        {
            if (!CanRestore(createdPages) || !expected.IsCurrent)
                throw new InvalidOperationException("Another navigation changed the stack; restoring it would overwrite unrelated navigation.");
        }

        // Direct MAUI navigation does not share Prism's semaphore. Advance an expected
        // topology for each owned mutation so outside pushes, pops and reorders are detected.
        EnsureOwnership();
        var commonModals = _window.Navigation.ModalStack.Zip(_modals)
            .TakeWhile(pair => pair.First == pair.Second).Count();
        while (_window.Navigation.ModalStack.Count > commonModals)
        {
            EnsureOwnership();
            var page = _window.Navigation.ModalStack.Last();
            expected._modals.Remove(page);
            if (await pop(_window.Navigation, true, false) != page)
                throw new NavigationException(NavigationException.UnsupportedMauiNavigation, page);
            EnsureOwnership();
        }

        EnsureOwnership();
        if (_window.Page != _root)
        {
            expected._root = _root;
            expected.Capture(_root);
            _window.Page = _root;
        }
        EnsureOwnership();
        foreach (var (flyout, state) in _flyouts)
        {
            EnsureOwnership();
            expected._flyouts[flyout] = state;
            expected.Capture(state.Detail);
            if (flyout.Detail != state.Detail)
                flyout.Detail = state.Detail;
            flyout.IsPresented = state.Presented;
            EnsureOwnership();
        }
        foreach (var (tabbed, selected) in _tabs)
        {
            EnsureOwnership();
            expected._tabs[tabbed] = selected;
            tabbed.CurrentPage = selected;
            EnsureOwnership();
        }

        foreach (var (navigationPage, originalPages) in _navigation)
        {
            var navigation = navigationPage.Navigation;
            var expectedPages = expected._navigation[navigationPage];
            for (var index = 0; index < originalPages.Count; index++)
            {
                EnsureOwnership();
                var page = originalPages[index];
                if (navigation.NavigationStack.Contains(page))
                    continue;
                var next = originalPages.Skip(index + 1).FirstOrDefault(navigation.NavigationStack.Contains);
                expected.Capture(page);
                if (next is not null)
                {
                    expectedPages.Insert(expectedPages.IndexOf(next), page);
                    navigation.InsertPageBefore(page, next);
                }
                else
                {
                    expectedPages.Add(page);
                    await navigation.PushAsync(page, false);
                }
                EnsureOwnership();
            }
            foreach (var page in navigation.NavigationStack.Reverse().ToArray())
            {
                EnsureOwnership();
                if (originalPages.Contains(page))
                    continue;
                expectedPages.Remove(page);
                if (navigationPage.CurrentPage == page)
                {
                    if (await pop(navigation, false, false) != page)
                        throw new NavigationException(NavigationException.UnsupportedMauiNavigation, page);
                }
                else
                    navigation.RemovePage(page);
                EnsureOwnership();
            }
        }

        foreach (var modal in _modals.Skip(commonModals))
        {
            EnsureOwnership();
            expected._modals.Add(modal);
            expected.Capture(modal);
            await _window.Navigation.PushModalAsync(modal, false);
            EnsureOwnership();
        }
    }
}
