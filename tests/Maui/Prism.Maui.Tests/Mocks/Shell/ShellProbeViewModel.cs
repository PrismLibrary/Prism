using Prism.Navigation;

namespace Prism.Maui.Tests.Mocks.Shell;

public sealed class ShellProbeViewModel : IInitialize, INavigatedAware, IQueryAttributable
{
    public ShellProbeViewModel(ShellProbeDependency dependency)
    {
        Dependency = dependency;
    }

    public ShellProbeDependency Dependency { get; }
    public int InitializationCount { get; private set; }
    public int NavigatedToCount { get; private set; }
    public int NavigatedFromCount { get; private set; }
    public IDictionary<string, object> Query { get; private set; }

    public void Initialize(INavigationParameters parameters)
    {
        InitializationCount++;
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        NavigatedToCount++;
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
        NavigatedFromCount++;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Query = new Dictionary<string, object>(query);
    }
}
