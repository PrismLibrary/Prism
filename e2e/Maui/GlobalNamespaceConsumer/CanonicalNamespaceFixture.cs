using Prism.Maui.Tests.Mocks;
using Prism.Xaml;

namespace GlobalNamespaceConsumer;

public class CanonicalNamespaceFixture
{
    [Fact]
    public void CanonicalMarkupWorksWithAndWithoutGlobalAggregation()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var page = new CanonicalPage();
        var parameter = Assert.IsAssignableFrom<Parameter>(page.FindByName<Button>("ParameterButton").CommandParameter);
        Assert.Equal("Message", parameter.Key);
        Assert.Equal("Canonical", parameter.Value);
    }
}
