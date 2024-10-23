namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

internal class MockXamlViewViewModel : BindableBase
{
    private string _test = "Initial Value";
    public string Test
    {
        get => _test;
        set => SetProperty(ref _test, value);
    }
}
