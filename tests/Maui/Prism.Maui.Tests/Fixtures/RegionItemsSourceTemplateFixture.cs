using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Prism.Maui.Tests.Mocks;
using Prism.Navigation.Regions.Adapters;

namespace Prism.Maui.Tests;

public class RegionItemsSourceTemplateFixture
{
    [Fact]
    public void ContentTracksReusedBindingContext()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var template = new RegionItemsSourceTemplate();
        var container = Assert.IsType<ContentView>(template.CreateContent());
        var first = new ContentView();
        var second = new ContentView();

        container.BindingContext = first;
        Assert.Same(first, container.Content);
        container.BindingContext = second;
        Assert.Same(second, container.Content);
        container.BindingContext = null;
        Assert.Null(container.Content);
    }
}
