namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public class MockRegionViewB : StackLayout, IMessageLabel
{
    public MockRegionViewB()
    {
        Message = new Label();
        Message.SetBinding(Label.TextProperty, new Binding("Message"));
        Add(Message);
    }

    public Label Message { get; }
}
