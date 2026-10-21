using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed class DefaultViewModel
{
    public DefaultViewModel(WindowLifetime lifetime, IContainerProvider container)
    {
        Lifetime = lifetime;
        Container = container;
    }

    public WindowLifetime Lifetime { get; }
    public IContainerProvider Container { get; }
}
