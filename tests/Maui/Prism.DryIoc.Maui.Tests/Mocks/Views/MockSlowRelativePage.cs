namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public class MockSlowRelativePage : ContentPage, IInitializeAsync
{
    public async Task InitializeAsync(INavigationParameters parameters)
    {
        // Outlast DestroyPage's deferred transition cleanup before the next
        // page in the forward route is created from the caller's scope.
        await Task.Delay(TimeSpan.FromMilliseconds(1000));
        parameters.GetValue<Action>("checkScope")();
    }
}
