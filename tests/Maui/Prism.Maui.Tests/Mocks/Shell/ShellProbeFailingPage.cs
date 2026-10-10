namespace Prism.Maui.Tests.Mocks.Shell;

public sealed class ShellProbeFailingPage : ContentPage
{
    public ShellProbeFailingPage(ShellProbeDependency dependency)
    {
        throw new InvalidOperationException("Shell probe page creation failed.");
    }
}
