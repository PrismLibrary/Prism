namespace Prism.Maui.Tests.Mocks.Shell;

public sealed class ShellProbeDependency : IDisposable
{
    public int DisposeCount { get; private set; }
    public bool ThrowOnDispose { get; set; }

    public void Dispose()
    {
        DisposeCount++;
        if (ThrowOnDispose)
            throw new InvalidOperationException("Shell probe dependency disposal failed.");
    }
}
