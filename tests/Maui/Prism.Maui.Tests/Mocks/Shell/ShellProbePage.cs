using Prism.Common;

namespace Prism.Maui.Tests.Mocks.Shell;

public sealed class ShellProbePage : ContentPage, IDestructible
{
    public ShellProbePage(ShellProbeDependency dependency)
    {
        Dependency = dependency;
    }

    public ShellProbeDependency Dependency { get; }
    public bool Destroyed { get; private set; }

    public void Destroy()
    {
        Destroyed = true;
    }
}
