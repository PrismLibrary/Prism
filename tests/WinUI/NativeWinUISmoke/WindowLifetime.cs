namespace NativeWinUISmoke;

/// <summary>A disposable scoped dependency, observed without disposing it in the harness.</summary>
public sealed class WindowLifetime : IDisposable
{
    public WindowLifetime(RootLifetime root) => Root = root;

    public RootLifetime Root { get; }
    public int DisposeCalls { get; private set; }

    public void Dispose() => DisposeCalls++;
}
