namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MockXamlView : ContentPage
{
	public MockXamlView()
	{
		InitializeComponent();
	}

    public Entry TestEntry => testEntry;
}
