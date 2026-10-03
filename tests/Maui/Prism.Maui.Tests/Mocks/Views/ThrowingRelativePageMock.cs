namespace Prism.Maui.Tests.Mocks.Views;

public class ThrowingRelativePageMock : ContentPage
{
    public ThrowingRelativePageMock() => throw new InvalidOperationException("Destination creation failed.");
}
