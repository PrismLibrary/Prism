namespace Prism.Maui.Tests.Mocks.Shell;

internal sealed class ShellProbeDispatcher : IDispatcher
{
    public bool IsDispatchRequired => false;

    public bool Dispatch(Action action)
    {
        action();
        return true;
    }

    public bool DispatchDelayed(TimeSpan delay, Action action)
    {
        return false;
    }

    public IDispatcherTimer CreateTimer()
    {
        throw new NotSupportedException("The managed Shell probe does not use native timers.");
    }
}
