using Prism.Behaviors;
using Prism.Maui.Tests.Mocks;
using Prism.Maui.Tests.Mocks.Views;
using Prism.Mvvm;
using Prism.Xaml;

namespace Prism.Maui.Tests.Fixtures.Xaml;

public class GlobalNamespaceFixture
{
    [Fact]
    public void CompiledGlobalNamespaceResolvesCanonicalPrismTypes()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
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
        var behaviors = page.FindByName<Button>("BehaviorButton").Behaviors;
        Assert.Equal(2, behaviors.Count);
        Assert.IsType<EventToCommandBehavior>(behaviors[0]);
        Assert.IsType<Mocks.Xaml.EventToCommandBehavior>(behaviors[1]);
    }
}
