using Prism.Maui.Tests.Mocks;
using Prism.Maui.Tests.Mocks.Views;
using Prism.Xaml;

namespace Prism.Maui.Tests.Fixtures.Xaml;

public class ParameterExtensionFixture
{
    public ParameterExtensionFixture() => DispatcherProvider.SetCurrent(TestDispatcher.Provider);

    [Fact]
    public void ButtonPassesParameterToBoundCommand()
    {
        Parameter receivedParameter = null;
        var executionCount = 0;
        var clickCommand = new DelegateCommand<Parameter>(parameter =>
        {
            receivedParameter = parameter;
            executionCount++;
        });
        var page = new MarkupPage { BindingContext = new { ClickCommand = clickCommand } };
        var button = page.FindByName<Button>("ParameterButton");

        Assert.NotNull(button);
        Assert.Same(clickCommand, button.Command);
        var commandParameter = Assert.IsAssignableFrom<Parameter>(button.CommandParameter);

        button.SendClicked();

        Assert.Equal(1, executionCount);
        Assert.Same(commandParameter, receivedParameter);
        Assert.Equal("Message", receivedParameter.Key);
        Assert.Equal("Hello World", receivedParameter.Value);
    }
}
