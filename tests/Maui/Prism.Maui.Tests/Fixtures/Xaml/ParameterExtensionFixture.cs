using Prism.Behaviors;
using Prism.Maui.Tests.Mocks;
using Prism.Maui.Tests.Mocks.Views;
using Prism.Mvvm;
using Prism.Xaml;

namespace Prism.Maui.Tests.Fixtures.Xaml;

public class ParameterExtensionFixture
{
    public ParameterExtensionFixture() => DispatcherProvider.SetCurrent(TestDispatcher.Provider);

#if PRISM_MAUI_GLOBAL_XMLNS
    [Fact]
    public void CompiledGlobalNamespacePreservesMauiControlsAndCanonicalPrismTypes()
    {
        var page = new GlobalNamespacePage();

        var global = Assert.IsAssignableFrom<Parameter>(page.FindByName<Button>("GlobalParameterButton").CommandParameter);
        Assert.Equal("Message", global.Key);
        Assert.Equal("Global", global.Value);
        var canonical = Assert.IsAssignableFrom<Parameter>(page.FindByName<Button>("CanonicalParameterButton").CommandParameter);
        Assert.Equal("Canonical", canonical.Value);
        Assert.Equal(ViewModelLocatorBehavior.Disabled, ViewModelLocator.GetAutowireViewModel(page));
        Assert.NotNull(page.FindByName<Button>("NavigationButton").Command);
        Assert.NotNull(page.FindByName<Button>("BackButton").Command);
        Assert.NotNull(page.FindByName<Button>("DialogButton").Command);
        Assert.IsType<EventToCommandBehavior>(Assert.Single(page.FindByName<Button>("BehaviorButton").Behaviors));
        var tabs = Assert.IsType<Microsoft.Maui.Controls.TabbedPage>(page.Resources["Tabs"]);
        Assert.Equal("Prism Tab", Prism.Navigation.Xaml.TabbedPage.GetTitle(Assert.Single(tabs.Children)));
    }
#endif

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
