namespace NativeWinUISmoke;

/// <summary>A container-owned singleton that must survive individual window closures.</summary>
public sealed class RootLifetime : IDisposable
{
    public int DisposeCalls { get; private set; }

    public void Dispose() => DisposeCalls++;
}
