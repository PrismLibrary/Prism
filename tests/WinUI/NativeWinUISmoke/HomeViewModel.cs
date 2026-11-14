using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed class HomeViewModel
{
    public HomeViewModel(IContainerProvider container)
    {
        Container = container;
    }

    public IContainerProvider Container { get; }
    public string Message => "Home view and view model resolved through the application container.";
}
