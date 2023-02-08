using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Mocks.Views;

public class VMLDisabledPageMock : ContentPage
{
    public VMLDisabledPageMock()
    {
        SetValue(ViewModelLocator.AutowireViewModelProperty, ViewModelLocatorBehavior.Disabled);
    }
}
