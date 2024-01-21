namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

internal class ForcedView : ContentPage
{
    public ForcedView()
    {
        ViewModelLocator.SetAutowireViewModel(this, ViewModelLocatorBehavior.Forced);
    }
}
