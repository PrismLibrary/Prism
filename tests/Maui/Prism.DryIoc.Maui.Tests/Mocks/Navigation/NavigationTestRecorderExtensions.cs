namespace Prism.DryIoc.Maui.Tests.Mocks.Navigation;

public static class NavigationTestRecorderExtensions
{
    public static IReadOnlyList<NavigationPop> GetPops(this INavigationService navigationService)
    {
        if (navigationService is not TestPageNavigationService testNav)
            throw new InvalidCastException();

        return testNav.Recorder.Pops;
    }

    public static IReadOnlyList<NavigationPush> GetPushes(this INavigationService navigationService)
    {
        if (navigationService is not TestPageNavigationService testNav)
            throw new InvalidCastException();

        return testNav.Recorder.Pushes;
    }
}
