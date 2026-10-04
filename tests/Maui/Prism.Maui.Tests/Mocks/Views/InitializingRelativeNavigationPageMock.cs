using Prism.Navigation;

namespace Prism.Maui.Tests.Mocks.Views;

public class InitializingRelativeNavigationPageMock : NavigationPage, IInitializeAsync
{
    public Task InitializeAsync(INavigationParameters parameters) =>
        parameters.TryGetValue<Func<Task>>("initializeAsync", out var initialize) ? initialize() : Task.CompletedTask;
}
