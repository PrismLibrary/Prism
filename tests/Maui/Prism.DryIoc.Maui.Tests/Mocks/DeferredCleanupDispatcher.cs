namespace Prism.DryIoc.Maui.Tests.Mocks;

internal sealed class DeferredCleanupDispatcher : IDispatcher, IDispatcherProvider
{
    private readonly Queue<Action> _pending = new();

    public bool IsDispatchRequired => false;

    public IDispatcher GetForCurrentThread() => this;

    public IDispatcherTimer CreateTimer() => TestDispatcher.Current.CreateTimer();

    public bool Dispatch(Action action) => true;

    public bool DispatchDelayed(TimeSpan delay, Action action)
    {
        _pending.Enqueue(action);
        return true;
    }

    public void RunPending()
    {
        var actions = _pending.ToArray();
        _pending.Clear();
        foreach (var action in actions)
            action();
    }
}
