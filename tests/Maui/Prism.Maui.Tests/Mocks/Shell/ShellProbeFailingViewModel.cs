namespace Prism.Maui.Tests.Mocks.Shell;

public sealed class ShellProbeFailingViewModel
{
    public ShellProbeFailingViewModel(ShellProbeDependency dependency)
    {
        throw new InvalidOperationException("Shell probe view model creation failed.");
    }
}
