namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

#if !NET11_0_OR_GREATER
[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
public partial class MockXamlView : ContentPage
{
	public MockXamlView()
	{
		InitializeComponent();
	}

    public Entry TestEntry => testEntry;
}
