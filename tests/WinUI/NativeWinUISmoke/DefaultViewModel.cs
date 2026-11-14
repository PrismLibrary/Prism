using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed class DefaultViewModel
{
    public DefaultViewModel(IContainerProvider container)
    {
        Container = container;
    }

    public IContainerProvider Container { get; }
}
