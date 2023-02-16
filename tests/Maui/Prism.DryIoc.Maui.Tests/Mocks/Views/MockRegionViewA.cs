namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public class MockRegionViewA : StackLayout, IMessageLabel
{
    public MockRegionViewA()
    {
        Message = new Label();
        Message.SetBinding(Label.TextProperty, new Binding("Message"));
        Add(Message);
    }

    public Label Message { get; }
}
