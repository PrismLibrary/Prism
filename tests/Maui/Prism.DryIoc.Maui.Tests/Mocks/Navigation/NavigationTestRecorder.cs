namespace Prism.DryIoc.Maui.Tests.Mocks.Navigation;

internal class NavigationTestRecorder
{
    private readonly List<NavigationPop> _pops = [];
    private readonly List<NavigationPush> _pushes = [];

    public IReadOnlyList<NavigationPop> Pops => _pops;
    public IReadOnlyList<NavigationPush> Pushes => _pushes;

    public void Push(NavigationPush push) =>
        _pushes.Add(push);

    public void Pop(NavigationPop pop) =>
        _pops.Add(pop);
}
