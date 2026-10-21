using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed class HomeViewModel
{
    public HomeViewModel(WindowLifetime lifetime, IContainerProvider container)
    {
        Lifetime = lifetime;
        Container = container;
    }

    public WindowLifetime Lifetime { get; }
    public IContainerProvider Container { get; }
    public string Message => "Home view and view model resolved through this window's Prism scope.";
}
