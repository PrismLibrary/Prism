namespace Prism.Maui.Tests.Mocks.Shell;

internal sealed class ShellProbeDispatcherProvider : IDispatcherProvider
{
    public IDispatcher GetForCurrentThread()
    {
        return new ShellProbeDispatcher();
    }
}
